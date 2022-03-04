using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Acciaio.Editor;

namespace Acciaio.Collections.Generic.Editor
{
    [CustomPropertyDrawer(typeof(Acciaio.Collections.Generic.Map<,>), true)]
    public class MapDrawer : UnityEditor.PropertyDrawer
    {
        private const int MARGIN = 2;
        private const int ICON_SIZE = 20;
        private const string ICON_NAME = "console.warnicon.sml";
        private const string LIST_NAME = "_serializedEntries";
        private const string KEY_NAME = "Key";
        private const string VALUE_NAME = "Value";
        private const string DUP_TOOLTIP = "This voice is a duplicate and will be ignored.";
        private const string KEY_DUP_NAME = "{0} [DUPLICATE]";

        private ReorderableList _list = null;

        private bool IsAlreadyPresent(SerializedProperty list, int index)
        {
            var entry = list.GetArrayElementAtIndex(index);
            var key = entry.FindPropertyRelative(KEY_NAME);

            bool found = false;
            for (int i = 0; i < index && !found; i++)
            {
                var previousEntry = list.GetArrayElementAtIndex(i);
                var previousKey = previousEntry.FindPropertyRelative(KEY_NAME);
                found = SerializedProperty.DataEquals(key, previousKey);
            }
            return found;
        }

        private ReorderableList RetrieveList(SerializedProperty property, GUIContent label) {
            if (_list == null) {
                _list = new ReorderableList(property.serializedObject, property, true, true, true, true);
                _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, label);
                _list.elementHeightCallback = index => 
                {
                    var entry = _list.serializedProperty.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);
                    return EditorGUI.GetPropertyHeight(key, true) + EditorGUI.GetPropertyHeight(value, true);
                };
                _list.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var entry = _list.serializedProperty.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);

                    string keyName = KEY_NAME;
                    string keyTooltip = "";
                    if (IsAlreadyPresent(property, index))
                    {
                        var iconRect = new Rect(rect.x - ICON_SIZE + MARGIN, rect.y + ICON_SIZE - MARGIN, ICON_SIZE, ICON_SIZE);
                        EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent(ICON_NAME, DUP_TOOLTIP));
                        keyName = string.Format(KEY_DUP_NAME, keyName);
                        keyTooltip = DUP_TOOLTIP;
                    }
                    var keyRect = new Rect(rect.x, rect.y + MARGIN, rect.width, EditorGUI.GetPropertyHeight(key, true));
                    EditorGUI.PropertyField(keyRect, key, new GUIContent(keyName, keyTooltip));

                    var valueRect = new Rect(rect.x, keyRect.y + keyRect.height, rect.width, EditorGUI.GetPropertyHeight(value, true));
                    EditorGUI.PropertyField(valueRect, value, new GUIContent(VALUE_NAME));
                };
                _list.onAddCallback = list => 
                {
                    list.serializedProperty.arraySize++;
                    var entry = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                    var key = entry.FindPropertyRelative(KEY_NAME);
                    var value = entry.FindPropertyRelative(VALUE_NAME);
                    key.SetToDefault();
                    value.SetToDefault();
                };
            }
            return _list;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedEntries = property.FindPropertyRelative(LIST_NAME);
            RetrieveList(serializedEntries, label).DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totHeight = 0f;

            var serializedEntries = property.FindPropertyRelative(LIST_NAME);
            totHeight += RetrieveList(serializedEntries, label).GetHeight();

            return totHeight;
        }
    }
}
