using System;
using System.Linq;
using System.Text;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(Id), true)]
    public class IdDrawer : PropertyDrawer
    {
        private const string ValueName = "_value";

        private const float ButtonWidth = 18;

        private const string GenButtonIcon = "d_Refresh";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ValueName));

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(ValueName);
            
            Rect construction = new(rect);
            construction.width -= ButtonWidth;

            EditorGUI.BeginDisabledGroup(property.GetPropertyType() == typeof(AutoId));
            EditorGUI.PropertyField(construction, valueProperty, label);
            EditorGUI.EndDisabledGroup();

            construction.x += construction.width;
            construction.width = ButtonWidth;

            if (GUI.Button(construction, EditorGUIUtility.IconContent(GenButtonIcon), EditorStyles.label)) 
                valueProperty.stringValue = Guid.NewGuid().ToString();
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative(ValueName);
            
            VisualElement root = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            TextField valueField = new(property.displayName)
            {
                value = valueProperty.stringValue,
                bindingPath = valueProperty.propertyPath,
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1
                }
            };
            valueField.SetEnabled(property.GetPropertyType() != typeof(AutoId));

            Image genButton = new()
            {
                image = EditorGUIUtility.IconContent(GenButtonIcon).image,
                style =
                {
                    flexGrow = 0,
                    flexShrink = 0,
                    width = ButtonWidth
                }
            };
            
            genButton.RegisterCallback<ClickEvent, SerializedProperty>(
                (_, prop) => prop.stringValue = Guid.NewGuid().ToString(),
                valueProperty
            );
            
            root.Add(valueField);
            root.Add(genButton);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(ReferenceId<>))]
    public class ReferenceIdDrawer : PropertyDrawer
    {
        private const string GuidName = "_assetGuid";
        private const string RefIdName = "<ReferencedId>k__BackingField";
        private const string ValueName = "_value";
        
        private const int DisplayIdMaxLength = 16;
        private const int DisplayIdTrimmedHalfLength = 5;

        private static readonly StringBuilder LabelBuilder = new();

        private static UnityEngine.Object GetObject(SerializedProperty valueProperty, SerializedProperty guidProperty, Type refType)
        {
            UnityEngine.Object obj = null;
            if (!string.IsNullOrEmpty(guidProperty.stringValue))
            {
                obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guidProperty.stringValue), refType);
                var idObj = (IIdentifiable)obj;
                if (idObj.Id != valueProperty.stringValue) valueProperty.stringValue = idObj.Id;
            }

            return obj;
        }

        private static string CalculateLabel(string defaultLabel, string id)
        {
            if (id is null) return defaultLabel;

            LabelBuilder.Append(defaultLabel)
                    .Append(" (<color=\"red\"><b>");
            
            if (id.Length > DisplayIdMaxLength)
            {
                LabelBuilder.Append(id[..DisplayIdTrimmedHalfLength])
                        .Append("...")
                        .Append(id[^DisplayIdTrimmedHalfLength..]);
            }
            else LabelBuilder.Append(id);

            var result = LabelBuilder.Append("</b></color>)").ToString();
            LabelBuilder.Clear();
            return result;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ValueName));
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(RefIdName).FindPropertyRelative(ValueName);
            var guidProperty = property.FindPropertyRelative(GuidName);
            var refType = property.GetPropertyType().GetGenericArguments()[0];
            var obj = GetObject(valueProperty, guidProperty, refType);
            
            var newLabel = CalculateLabel(label.text, ((IIdentifiable)obj)?.Id);
            EditorStyles.label.richText = true;
            var newObj = EditorGUI.ObjectField(rect, newLabel, obj, refType, false);
            EditorStyles.label.richText = false;

            if (ReferenceEquals(newObj, obj)) return;
            
            valueProperty.stringValue = ((IIdentifiable)newObj)?.Id;
            guidProperty.stringValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newObj)).ToString();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative(RefIdName).FindPropertyRelative(ValueName);
            var guidProperty = property.FindPropertyRelative(GuidName);
            var refType = property.GetPropertyType().GetGenericArguments()[0];
            var obj = GetObject(valueProperty, guidProperty, refType);

            ObjectField field = new(CalculateLabel(property.displayName, ((IIdentifiable)obj)?.Id))
            {
                value = obj,
                allowSceneObjects = false,
                objectType = refType
            };

            field.RegisterValueChangedCallback(evt =>
            {
                var newObj = evt.newValue;
                if (ReferenceEquals(newObj, obj)) return;
                
                var newLabel = CalculateLabel(property.displayName, ((IIdentifiable)newObj)?.Id);
                field.label = newLabel;
                
                valueProperty.stringValue = ((IIdentifiable)newObj)?.Id;
                guidProperty.stringValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newObj)).ToString();
            });
            
            return field;
        }
    }
}