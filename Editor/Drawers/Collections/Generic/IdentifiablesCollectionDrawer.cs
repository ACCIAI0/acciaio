using Acciaio.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor.Collections.Generic
{
    [CustomPropertyDrawer(typeof(IdentifiablesCollection<>), true)]
    public sealed class IdentifiablesCollectionDrawer : PropertyDrawer
    {
        private const string ListPropertyName = "_list";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ListPropertyName));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            => EditorGUI.PropertyField(position, property.FindPropertyRelative(ListPropertyName), label);

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new PropertyField(property.FindPropertyRelative(ListPropertyName))
            {
                label = property.displayName
            };
        }
    }
}