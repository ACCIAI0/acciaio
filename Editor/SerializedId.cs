using Acciaio.Editor.Extensions;
using UnityEditor;

namespace Acciaio.Editor
{
    public sealed class SerializedId
    {
        private const string ValuePropertyName = "_value";

        private readonly SerializedProperty _property;
        private readonly SerializedProperty _valueProperty;

        public string StringValue
        {
            get => _valueProperty.stringValue;
            set => _valueProperty.stringValue = value;
        }

        public Id IdValue
        {
            get => _property.GetValue<Id>();
            set => StringValue = value;
        }

        public string PropertyPath => _valueProperty.propertyPath;
        
        public float Height => EditorGUI.GetPropertyHeight(_valueProperty);
        
        internal SerializedId(SerializedProperty property)
        {
            _property = property;
            _valueProperty = property.FindPropertyRelative(ValuePropertyName);
        }

        public SerializedProperty AsSerializedProperty() => _valueProperty;
    }
}