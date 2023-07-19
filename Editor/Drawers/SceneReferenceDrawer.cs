using System;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public sealed class SceneReferenceDrawer : PropertyDrawer
    {
        private readonly struct PropertiesTuple
        {
            private const string GuidName = "_assetGuid";
            private const string PathName = "_path";
            private const string IsEditorOverrideableName = "<IsEditorOverrideable>k__BackingField";
            private const string IsAddressableName = "<IsAddressable>k__BackingField";

            private readonly SerializedProperty _propertyGuid;
            private readonly SerializedProperty _propertyPath;
            private readonly SerializedProperty _propertyIsEditorOverrideable;
            private readonly SerializedProperty _propertyIsAddressable;
            
            public string DisplayNameOfParentProperty { get; }

            public bool CanBeOverriddenInEditor { get; }

            public bool IsPrefabOverride => _propertyGuid.prefabOverride;
            
            public GUID Guid
            {
                get => new(_propertyGuid.stringValue);
                set => _propertyGuid.stringValue = value.ToString();
            }
            
            public string Path
            {
                get => _propertyPath.stringValue;
                set => _propertyPath.stringValue = value;
            }

            public bool IsEditorOverrideable
            {
                get => _propertyIsEditorOverrideable.boolValue;
                set => _propertyIsEditorOverrideable.boolValue = value;
            }

            public bool CanBeAddressable => _propertyIsAddressable is not null;

            public bool IsAddressable
            {
                get => _propertyIsAddressable?.boolValue ?? false;
                set
                {
                    if (!CanBeAddressable) return;
                    _propertyIsAddressable.boolValue = value;
                }
            }

            public PropertiesTuple(SerializedProperty property, string displayName = null)
            {
                _propertyGuid = property.FindPropertyRelative(GuidName);
                _propertyPath = property.FindPropertyRelative(PathName);
                _propertyIsEditorOverrideable = property.FindPropertyRelative(IsEditorOverrideableName);
                _propertyIsAddressable = property.FindPropertyRelative(IsAddressableName);

                DisplayNameOfParentProperty = displayName ?? property.displayName;
                CanBeOverriddenInEditor = property.GetAttribute<AllowEditorOverrideAttribute>() is not null;
            }

            public void SerializeAsset(SceneAsset asset)
            {
                Path = AssetDatabase.GetAssetPath(asset);
                Guid = AssetDatabase.GUIDFromAssetPath(Path);
                _propertyPath.serializedObject.ApplyModifiedProperties();
            }
        }
        
        private const int IconWidth = 18;
        
        private const string IsEditorOverrideableTooltip =
            "If active (white), this scene will be overridden with the active scene before entering playmode";
        
        private const string InvalidTooltip =
            "This scene is neither flagged as Addressables or present in the Build Order";
        private const string InactiveTooltip = "The scene is present in the Build Order but it's disabled";
        private const string NullSceneTooltip = "No scene specified";
        
        private const string AddressableTooltip = "This scene will be loaded using the Addressables APIs";
        private const string BuildSettingsTooltip = "This scene will be loaded from the build settings";

        private const string UnityOnIcon = "UnityLogo";
        private const string UnityOffIcon = "UnityLogoLarge";
        private const string AddressablesIconOn = "d_CacheServerConnected";
        private const string AddressablesIconOff = "d_CacheServerDisabled";
        private const string InvalidIcon = "d_redLight";
        private const string ValidButDisabledIcon = "d_orangeLight";
        private const string ValidIcon = "d_greenLight";

        private static EditorBuildSettingsScene GetSceneInBuildSettings(SceneAsset asset)
        {
            if (asset == null) return null;
            
            EditorBuildSettingsScene scene = null;
            foreach (var buildSettingsScene in EditorBuildSettings.scenes)
            {
                if (!buildSettingsScene.path.Equals(AssetDatabase.GetAssetPath(asset), StringComparison.Ordinal)) 
                    continue;
                
                scene = buildSettingsScene;
                break;
            }

            return scene;
        }

        private static string GetAddressablesIconName(bool value) => value ? AddressablesIconOn : AddressablesIconOff;

        private static string GetUnityIconName(bool value) => value ? UnityOnIcon : UnityOffIcon;

        private static string GetValidityTooltip(SceneAsset asset)
        {
            if (asset is null) return NullSceneTooltip;

            var settingsScene = GetSceneInBuildSettings(asset);
            if (settingsScene is not null) return settingsScene.enabled ? null: InactiveTooltip;
            return IsSceneAddressable(asset) ? null : InvalidTooltip;
        }

        private static string GetValidityIconName(SceneAsset asset)
        {
            if (asset is null) return InvalidIcon;

            var settingsScene = GetSceneInBuildSettings(asset);

            if (settingsScene is not null) return settingsScene.enabled ? ValidIcon : ValidButDisabledIcon;
            return IsSceneAddressable(asset) ? ValidIcon : InvalidIcon;
        }

        private static bool IsSceneAddressable(SceneAsset asset)
        {
            if (asset == null) return false;
            
#if USE_ADDRESSABLES
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return false;
            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)));
            return entry != null;
#else
            return false;
#endif
        }

        private static SceneAsset RetrieveSceneAsset(PropertiesTuple properties)
        {
            if (properties.Guid.Empty()) return null;

            var assetPath = AssetDatabase.GUIDToAssetPath(properties.Guid);

            if (assetPath is null)
            {
                properties.Guid = new(string.Empty);
                properties.Path = string.Empty;
                return null;
            }

            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            if (!properties.Path.Equals(assetPath, StringComparison.Ordinal))
                properties.SerializeAsset(asset);

            return asset;
        }
        
        private static VisualElement CreateValuesFields(PropertiesTuple properties)
        {
            VisualElement values = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    flexShrink = 1
                }
            };

            var asset = RetrieveSceneAsset(properties);
            
            properties.IsAddressable = IsSceneAddressable(asset);
            
            var validityIconName = GetValidityIconName(asset);
            Image validityIcon = new()
            {
                tooltip = GetValidityTooltip(asset),
                image = EditorGUIUtility.IconContent(validityIconName).image,
                style =
                {
                    width = IconWidth,
                    height = IconWidth + 2
                }
            };

            ObjectField sceneAsset = new(properties.DisplayNameOfParentProperty)
            {
                allowSceneObjects = false,
                objectType = typeof(SceneAsset),
                value = asset,
                style =
                {
                    unityFontStyleAndWeight = properties.IsPrefabOverride ? FontStyle.Bold : FontStyle.Normal,
                    flexGrow = 1,
                    flexShrink = 1,
                    overflow = Overflow.Hidden
                }
            };
            sceneAsset.Q<Label>().style.unityFontStyleAndWeight = 
                    properties.IsPrefabOverride ? FontStyle.Bold : FontStyle.Normal;

            Image isOverrideableToggle = new()
            {
                image = EditorGUIUtility.IconContent(GetUnityIconName(properties.IsEditorOverrideable)).image,
                tooltip = IsEditorOverrideableTooltip
            };

            Image isAddressableIcon = new()
            {
                tooltip = properties.IsAddressable ? AddressableTooltip : BuildSettingsTooltip,
                image = EditorGUIUtility.IconContent(GetAddressablesIconName(properties.IsAddressable)).image,
                style =
                {
                    width = IconWidth,
                    maxHeight = IconWidth + 2
                }
            };
            
            isOverrideableToggle.RegisterCallback<ClickEvent, Image>((_, me) =>
            {
                properties.IsEditorOverrideable = !properties.IsEditorOverrideable;
                isOverrideableToggle.style.color = properties.IsEditorOverrideable ? Color.cyan : Color.white;
            }, isOverrideableToggle);

            sceneAsset.RegisterValueChangedCallback(evt =>
            {
                var scene = evt.newValue as SceneAsset;
                properties.IsAddressable = IsSceneAddressable(scene);
                properties.SerializeAsset(scene);
                
                var iconName = GetAddressablesIconName(properties.IsAddressable);
                isAddressableIcon.tooltip = properties.IsAddressable ? AddressableTooltip : BuildSettingsTooltip;
                isAddressableIcon.image = EditorGUIUtility.IconContent(iconName).image;
                
                iconName = GetValidityIconName(scene);
                validityIcon.tooltip = GetValidityTooltip(scene);
                validityIcon.image = EditorGUIUtility.IconContent(iconName).image;
                
                sceneAsset.style.unityFontStyleAndWeight =
                    properties.IsPrefabOverride ? FontStyle.Bold : FontStyle.Normal;
                sceneAsset.Q<Label>().style.unityFontStyleAndWeight = 
                    properties.IsPrefabOverride ? FontStyle.Bold : FontStyle.Normal;
            });
            
            values.Add(validityIcon);
            values.Add(sceneAsset);
            if (properties.CanBeAddressable) values.Add(isAddressableIcon);
            if (properties.CanBeOverriddenInEditor) values.Add(isOverrideableToggle);

            return values;
        }

        public static bool UpdateProperty(SerializedProperty property)
        {
            if (property.GetPropertyType() != typeof(SceneReference))
            {
                Debug.LogError($"Property {property.displayName} is not of type SceneReference");
                return false;
            }

            var tuple = new PropertiesTuple(property);
            var path = tuple.Path;
            RetrieveSceneAsset(tuple);
            return !path.Equals(tuple.Path, StringComparison.Ordinal);
        }

        public VisualElement CreatePropertyGUI(SerializedProperty property, string displayName) 
            => CreateValuesFields(new PropertiesTuple(property, displayName));

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => CreatePropertyGUI(property, null);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            PropertiesTuple properties = new(property, label.text);

            var fieldWidth = rect.width - IconWidth;
            if(properties.CanBeOverriddenInEditor) fieldWidth -= IconWidth;
            if (properties.CanBeAddressable) fieldWidth -= IconWidth;

            var scene = RetrieveSceneAsset(properties);
            
            properties.IsAddressable = IsSceneAddressable(scene);
            
            Rect constructionRect = new(rect)
            {
                width = IconWidth
            };
            var iconName = GetValidityIconName(scene);
            var tooltip = GetValidityTooltip(scene);
            EditorGUI.LabelField(constructionRect, new GUIContent(null, EditorGUIUtility.IconContent(iconName).image, tooltip));

            constructionRect.x += constructionRect.width;
            constructionRect.width = fieldWidth;

            EditorGUIUtility.labelWidth -= IconWidth;

            if (properties.IsPrefabOverride)
            {
                EditorStyles.objectField.fontStyle = FontStyle.Bold;
                EditorStyles.label.fontStyle = FontStyle.Bold;
            }
            var newScene =
                    EditorGUI.ObjectField(constructionRect, label, scene, typeof(SceneAsset), false) as SceneAsset;
            EditorStyles.objectField.fontStyle = FontStyle.Normal;
            EditorStyles.label.fontStyle = FontStyle.Normal;
            EditorGUIUtility.labelWidth = 0;

            if (newScene != scene)
            {
                properties.IsAddressable = IsSceneAddressable(newScene);
                properties.SerializeAsset(newScene);
            }

            if (properties.CanBeAddressable)
            {
                iconName = GetAddressablesIconName(properties.IsAddressable);
                constructionRect.x += constructionRect.width;
                constructionRect.width = IconWidth;
                tooltip = properties.IsAddressable ? AddressableTooltip : BuildSettingsTooltip;
                EditorGUI.LabelField(constructionRect, new GUIContent(null, EditorGUIUtility.IconContent(iconName).image, tooltip));
            }

            if (properties.CanBeOverriddenInEditor)
            {
                constructionRect.x += constructionRect.width;
                constructionRect.width = IconWidth;
                var icon = EditorGUIUtility.IconContent(GetUnityIconName(properties.IsEditorOverrideable)).image;
                GUIContent content = new(null,icon, IsEditorOverrideableTooltip);
                if (GUI.Button(constructionRect, content, EditorStyles.label))
                {
                    properties.IsEditorOverrideable = !properties.IsEditorOverrideable;
                }
            }
        }
    }
}