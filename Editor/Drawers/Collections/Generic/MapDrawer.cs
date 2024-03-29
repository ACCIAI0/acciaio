using System.Collections.Generic;
using System.Reflection;
using Acciaio.Collections.Generic;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor.Collections.Generic
{
    [CustomPropertyDrawer(typeof(MapBase<,,>), true)]
    public sealed class MapDrawer : PropertyDrawer
    {
        private const int IconSize = 18;
        private const int Padding = 2;
        private const int CountFieldOffset = 18;
        private const int CountFieldWidth = 48;
        private const string WarnIconName = "console.warnicon@2x";
        private const string ErrIconName = "console.erroricon@2x";
        private const string ListName = "_serializedEntries";
        private const string KeyName = "Key";
        private const string ValueName = "Value";
        private const string DupTooltip = "This entry is a duplicate and will be ignored.";
        private const string NullTooltip = "A key with a null value will be ignored.";
        private const string KeyDupName = "<color=yellow>{0} [DUPLICATE]</color>";
        private const string KeyNullName = "<color=red>{0} [NULL]</color>";

        private readonly Dictionary<string, ReorderableList> _lists = new();

        private static bool IsAlreadyPresent(SerializedProperty list, int index)
        {
            var entry = list.GetArrayElementAtIndex(index);
            var key = entry.FindPropertyRelative(KeyName);

            var found = false;
            for (var i = 0; i < index && !found; i++)
            {
                var previousEntry = list.GetArrayElementAtIndex(i);
                var previousKey = previousEntry.FindPropertyRelative(KeyName);
                found = SerializedProperty.DataEquals(key, previousKey);
            }
            return found;
        }

        private ReorderableList RetrieveList(SerializedProperty property, GUIContent label)
        {
            if (_lists.ContainsKey(property.propertyPath)) return _lists[property.propertyPath];

            var names = fieldInfo.GetCustomAttribute<MapNamesAttribute>();
            ReorderableList list = new(property.serializedObject, property, true, false, true, true)
            {
                headerHeight = 0,
                elementHeightCallback = index =>
                {
                    if (property.arraySize == 0) return EditorGUIUtility.singleLineHeight;
                    var entry = property.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KeyName);
                    var value = entry.FindPropertyRelative(ValueName);
                    return EditorGUI.GetPropertyHeight(key, true) + EditorGUI.GetPropertyHeight(value, true) + 4;
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    rect.height -= 2;

                    var entry = property.GetArrayElementAtIndex(index);
                    var key = entry.FindPropertyRelative(KeyName);
                    var value = entry.FindPropertyRelative(ValueName);

                    var keyName = names?.KeyName ?? MapNamesAttribute.DefaultKey;
                    var keyTooltip = "";

                    var isNullKey = key.propertyType == SerializedPropertyType.ObjectReference && key.objectReferenceValue == null;
                    if (isNullKey || IsAlreadyPresent(property, index))
                    {
                        var iconRect = new Rect(rect.x - IconSize, rect.y, IconSize,
                            IconSize);
                        var icon = isNullKey ? ErrIconName : WarnIconName;
                        keyTooltip = isNullKey ? NullTooltip : DupTooltip;
                        EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent(icon, keyTooltip));
                        keyName = string.Format(isNullKey ? KeyNullName : KeyDupName, keyName);
                    }

                    var keyRect = new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(key, true));
                    EditorStyles.label.richText = true;
                    EditorGUI.PropertyField(keyRect, key, new GUIContent(keyName, keyTooltip), true);
                    EditorStyles.label.richText = false;

                    var valueRect = new Rect(rect.x, keyRect.y + keyRect.height, rect.width,
                        EditorGUI.GetPropertyHeight(value, true));
                    EditorGUI.PropertyField(valueRect, value, new GUIContent(names?.ValueName ?? MapNamesAttribute.DefaultValue), true);
                },
                onAddCallback = list =>
                {
                    list.serializedProperty.arraySize++;
                    var entry = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                    var key = entry.FindPropertyRelative(KeyName);
                    var value = entry.FindPropertyRelative(ValueName);
                    key.SetToDefault();
                    value.SetToDefault();
                }
            };
            _lists.Add(property.propertyPath, list);
            return list;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            
            var totHeight = 0f;

            var serializedEntries = property.FindPropertyRelative(ListName);
            totHeight += RetrieveList(serializedEntries, label).GetHeight();

            return totHeight + EditorGUIUtility.singleLineHeight + Padding;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedEntries = property.FindPropertyRelative(ListName);
            
            var foldoutRect = new Rect(position)
            {
                width = position.width - CountFieldWidth,
                height = EditorGUIUtility.singleLineHeight
            };

            var countRect = new Rect(position)
            {
                x = position.width - CountFieldWidth + CountFieldOffset,
                width = CountFieldWidth,
                height = EditorGUIUtility.singleLineHeight
            };

            var foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true, foldoutStyle);
            
            serializedEntries.arraySize = EditorGUI.IntField(countRect, GUIContent.none, serializedEntries.arraySize);
            
            if (!property.isExpanded)
            {
                property.GetValue<ISerializationCallbackReceiver>().OnAfterDeserialize();
                return;
            }

            position.y += EditorGUIUtility.singleLineHeight + Padding;
            position.height -= foldoutRect.height;
            RetrieveList(serializedEntries, label).DoList(position);
            property.GetValue<ISerializationCallbackReceiver>().OnAfterDeserialize();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
             var serializedEntries = property.FindPropertyRelative(ListName);
            IMGUIContainer container = new()
            {
                onGUIHandler = () =>
                {
                    RetrieveList(serializedEntries, new GUIContent(ObjectNames.NicifyVariableName(property.name))).DoLayoutList();
                    property.GetValue<ISerializationCallbackReceiver>().OnAfterDeserialize();
                }
            };
            return container;
        }
    }
}
