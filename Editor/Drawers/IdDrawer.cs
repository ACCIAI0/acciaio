using System;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(Id), true)]
    public class IdDrawer : PropertyDrawer
    {
        private const string ValueName = "_value";

        private const float ButtonWidth = 25;

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
}