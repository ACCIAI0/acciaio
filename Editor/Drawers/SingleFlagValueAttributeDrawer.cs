using System;
using System.Linq;
using System.Reflection;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(SingleFlagValueAttribute))]
    public sealed class SingleFlagValueAttributeDrawer : PropertyDrawer
    {
        private const string WrongFieldTypeMessage =
            "SingleFlagValue Attribute is not assigned to an enum field/property.";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogWarning(WrongFieldTypeMessage);
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var propertyType = property.GetPropertyType();
            
            if (propertyType.GetCustomAttribute<FlagsAttribute>() == null)
            {
                Debug.LogWarning($"{propertyType.Name} is not marked as Flags");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var values = Enum.GetValues(propertyType);

            if (values.Length == 0)
            {
                EditorGUI.LabelField(position, $"{property.displayName} is a {propertyType.Name}, which has values");
                return;
            }
            
            var intValues = values.Cast<int>().ToArray();
            var labels = values.Cast<Enum>().Select(e => e.ToString()).ToArray();

            if (!intValues.Contains(property.intValue)) property.intValue = intValues[0];

            property.intValue = EditorGUI.IntPopup(position, label.text, property.intValue, labels, intValues);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogWarning(WrongFieldTypeMessage);
                return new PropertyField(property);
            }
            
            var propertyType = property.GetPropertyType();
            if (propertyType.GetCustomAttribute<FlagsAttribute>() == null)
            {
                Debug.LogWarning($"{propertyType.Name} is not marked as Flags");
                return new PropertyField(property);
            }
            
            var values = Enum.GetValues(property.GetPropertyType()).Cast<Enum>().ToList();
            PopupField<Enum> popup = new(values, property.enumValueIndex)
            {
                label = property.displayName
            };

            popup.RegisterValueChangedCallback(evt => property.enumValueIndex = values.IndexOf(evt.newValue));

            return popup;
        }
    }
}