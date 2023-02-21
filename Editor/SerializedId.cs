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

    public sealed class SerializedReferenceId
    {
        private const string ValuePropertyName = "<ReferencedId>k__BackingField";
        private const string GuidPropertyName = "_assetGuid";
        
        private readonly SerializedProperty _guidProperty;
        private readonly SerializedId _valueProperty;

        internal string GuidValue
        {
            get => _guidProperty.stringValue;
            set => _guidProperty.stringValue = value;
        }
        
        public string StringValue
        {
            get => _valueProperty.StringValue;
            set => _valueProperty.StringValue = value;
        }

        public Id IdValue
        {
            get => _valueProperty.IdValue;
            set => _valueProperty.IdValue = value;
        }

        public float Height => _valueProperty.Height;
        
        internal SerializedReferenceId(SerializedProperty property)
        {
            _guidProperty = property.FindPropertyRelative(GuidPropertyName);
            _valueProperty = property.FindIdRelative(ValuePropertyName);
        }

        public SerializedProperty AsSerializedProperty() => _valueProperty.AsSerializedProperty();
    }
}