using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Acciaio.Editor;
using System.Reflection;
using UnityEngine.UIElements;

namespace Acciaio.Collections.Generic.Editor
{
    [CustomPropertyDrawer(typeof(Map<,>), true)]
    public sealed class MapDrawer : PropertyDrawer
    {
        private const int MARGIN = 2;
        private const int ICON_SIZE = 20;
        private const string WARN_ICON_NAME = "console.warnicon.sml";
        private const string ERR_ICON_NAME = "console.erroricon.sml";
        private const string LIST_NAME = "_serializedEntries";
        private const string KEY_NAME = "Key";
        private const string VALUE_NAME = "Value";
        private const string DUP_TOOLTIP = "This voice is a duplicate and will be ignored.";
        private const string NULL_TOOLTIP = "A key with a null value will be ignored.";
        private const string KEY_DUP_NAME = "<color=yellow>{0} [DUPLICATE]</color>";
        private const string KEY_NULL_NAME = "<color=red>{0} [NULL]</color>";

        private ReorderableList _list = null;

        private static bool IsAlreadyPresent(SerializedProperty list, int index)
        {
            var entry = list.GetArrayElementAtIndex(index);
            var key = entry.FindPropertyRelative(KEY_NAME);

            var found = false;
            for (var i = 0; i < index && !found; i++)
            {
                var previousEntry = list.GetArrayElementAtIndex(i);
                var previousKey = previousEntry.FindPropertyRelative(KEY_NAME);
                found = SerializedProperty.DataEquals(key, previousKey);
            }
            return found;
        }

        private ReorderableList RetrieveList(SerializedProperty property, GUIContent label)
        {
            if (_list != null) return _list;

            var names = fieldInfo.GetCustomAttribute<MapNames>();
            _list = new ReorderableList(property.serializedObject, property, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, label),
                elementHeightCallback = index =>
                {
                    if (_list.serializedProperty.arraySize == 0) return EditorGUIUtility.singleLineHeight;
                    var entry = _list.serializedProperty.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);
                    return EditorGUI.GetPropertyHeight(key, true) + EditorGUI.GetPropertyHeight(value, true);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var entry = _list.serializedProperty.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);

                    string keyName = names?.KeyName ?? MapNames.DEFAULT_KEY;
                    string keyTooltip = "";

                    bool isNullKey = key.propertyType == SerializedPropertyType.ObjectReference && key.objectReferenceValue == null;
                    if (isNullKey || IsAlreadyPresent(property, index))
                    {
                        var iconRect = new Rect(rect.x - ICON_SIZE + MARGIN, rect.y + ICON_SIZE - MARGIN, ICON_SIZE,
                            ICON_SIZE);
                        var icon = isNullKey ? ERR_ICON_NAME : WARN_ICON_NAME;
                        keyTooltip = isNullKey ? NULL_TOOLTIP : DUP_TOOLTIP;
                        EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent(icon, keyTooltip));
                        keyName = string.Format(isNullKey ? KEY_NULL_NAME : KEY_DUP_NAME, keyName);
                    }

                    var keyRect = new Rect(rect.x, rect.y + MARGIN, rect.width, EditorGUI.GetPropertyHeight(key, true));
                    EditorStyles.label.richText = true;
                    EditorGUI.PropertyField(keyRect, key, new GUIContent(keyName, keyTooltip), true);
                    EditorStyles.label.richText = false;

                    var valueRect = new Rect(rect.x, keyRect.y + keyRect.height, rect.width,
                        EditorGUI.GetPropertyHeight(value, true));
                    EditorGUI.PropertyField(valueRect, value, new GUIContent(names?.ValueName ?? MapNames.DEFAULT_VALUE), true);
                },
                onAddCallback = list =>
                {
                    list.serializedProperty.arraySize++;
                    var entry = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);
                    key.SetToDefault();
                    value.SetToDefault();
                }
            };
            return _list;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totHeight = 0f;

            var serializedEntries = property.FindPropertyRelative(LIST_NAME);
            totHeight += RetrieveList(serializedEntries, label).GetHeight();

            return totHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedEntries = property.FindPropertyRelative(LIST_NAME);
            RetrieveList(serializedEntries, label).DoList(position);

            property.GetValue<ISerializationCallbackReceiver>().OnAfterDeserialize();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            IMGUIContainer container = new();
            container.style.flexGrow = 1;
            container.style.flexShrink = 1;
            container.onGUIHandler = () => OnGUI(container.contentRect, property, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
            return container;
        }
    }
}
