using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acciaio.Collections;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(TypeReference<>))]
    public class TypeReferenceDrawer : PropertyDrawer
    {
        private const string StringPropertyName = "_assemblyQualifiedName";

        private static List<Type> GetPossibleTypes(SerializedProperty property)
        {
            var baseType = property.GetPropertyType().GenericTypeArguments[0];
            var types = TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract).ToList();
            
            if (!baseType.IsAbstract) types.Insert(0, baseType);

            return types;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
            => EditorGUIUtility.singleLineHeight;
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var stringProperty = property.FindPropertyRelative(StringPropertyName);
            
            var types = GetPossibleTypes(property);
            var displayOptions = types.Select(t => new GUIContent(t.FullName)).ToArray();
            
            var index = Mathf.Max(types.IndexOf(Type.GetType(stringProperty.stringValue)), 0);
            var newIndex = EditorGUI.Popup(rect, label, index, displayOptions);

            if (newIndex != index) stringProperty.stringValue = types[newIndex].AssemblyQualifiedName;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var stringProperty = property.FindPropertyRelative(StringPropertyName);
            var types = GetPossibleTypes(property);
            var initialValue = Type.GetType(stringProperty.stringValue) ?? types[0];

            PopupField<Type> field = new(types, initialValue)
            {
                formatSelectedValueCallback = type => type.Name,
                formatListItemCallback = type => type.FullName,
                label = property.displayName
            };
            
            field.RegisterValueChangedCallback(evt =>
                stringProperty.stringValue = evt.newValue.AssemblyQualifiedName);

            return field;
        }
    }
}
