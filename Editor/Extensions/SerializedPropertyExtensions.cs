using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Acciaio.Editor.Extensions
{
    public static class SerializedPropertyExtensions
    {
		private const BindingFlags InstanceAny = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        private const string PathElementArray = "Array";

		private static void SetDefaultConstructorValue(SerializedProperty property)
		{
			var parent = GetObject(property, true);
			var field = parent.GetType().GetField(property.name,  InstanceAny);
			field.SetValue(parent, Activator.CreateInstance(field.FieldType, true));
		}

        private static object GetObject(SerializedProperty property, bool stopAtParent)
        {
            var sObj = property.serializedObject;
            sObj.ApplyModifiedProperties();
            object @object = sObj.targetObject;

            var elements = property.propertyPath.Split('.');
            for (var i = 0; i < elements.Length - Convert.ToInt32(stopAtParent); i++)
            {
                var pathElement = elements[i];

                if (pathElement == PathElementArray)
                {
                    var index = int.Parse(elements[i + 1].Replace("data[", "").Replace("]", ""));
                    var j = -1;
                    var enumerator = ((IEnumerable)@object).GetEnumerator();
                    while(j < index && enumerator.MoveNext()) j++;
                    if (j == index) @object = enumerator.Current;
                    else 
                    {
                        Debug.LogError("Serialized object is referencing an item outside of a collection's size");
                        return null;
                    }
                    i++;
                }
                else
                {
					var type = @object.GetType();
					FieldInfo field;
					do
					{
						field = type.GetField(pathElement, InstanceAny);
						if (field == null) type = type.BaseType;
					} while (field == null && type != typeof(object));
                    @object = field?.GetValue(@object);
                }
            }
            return @object;
        }

        public static SerializedId FindIdRelative(this SerializedProperty property, string path)
        {
	        var idProperty = property.FindPropertyRelative(path);

	        if (idProperty is null) return null;
	        if (idProperty.type != nameof(Id) && idProperty.type != nameof(AutoId))
	        {
		        Debug.LogError($"Property of type {idProperty.type} is being extracted as IdProperty.");
		        return null;
	        }

	        return new(idProperty);
        }
        
        public static SerializedReferenceId FindReferenceIdRelative(this SerializedProperty property, string path)
        {
	        var refProperty = property.FindPropertyRelative(path);

	        if (refProperty is null) return null;
	        if (refProperty.type != nameof(Id) && refProperty.type != typeof(IdReference<>).Name)
	        {
		        Debug.LogError($"Property of type {refProperty.type} is being extracted as ReferenceIdProperty.");
		        return null;
	        }

	        return new(refProperty);
        }

        public static Gradient GetGradientValue(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Gradient) return null;
            return property.GetType()
                    .GetProperty("gradientValue", InstanceAny)
                    .GetValue(property) as Gradient;
        }

        public static void SetGradientValue(this SerializedProperty property, Gradient gradient)
        {
            if (property.propertyType != SerializedPropertyType.Gradient)
            {
                Debug.LogError("type is not a supported Gradient type");
                return;
            }

            property.GetType()
                    .GetProperty("gradientValue", InstanceAny)
                    .SetValue(property, gradient);
        }

		public static Type GetPropertyType(this SerializedProperty property) => GetObject(property, false)?.GetType();

        public static T GetValue<T>(this SerializedProperty property)
        {
            var @object = GetObject(property, false);
            if (@object is T tObject) return tObject;
            return default;
        }

        public static void SetToDefault(this SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.ArraySize: 
					property.arraySize = 0;
					break;
				case SerializedPropertyType.Boolean: 
					property.boolValue = false;
					break;
				case SerializedPropertyType.Bounds: 
					property.boundsValue = default;
					break;
				case SerializedPropertyType.BoundsInt: 
					property.boundsIntValue = default;
					break;
				case SerializedPropertyType.Character: 
					property.stringValue = "" + default(char);
					break;
				case SerializedPropertyType.Color: 
					property.colorValue = default;
					break;
				case SerializedPropertyType.Enum: 
					property.enumValueIndex = 0;
					break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = null;
                    break;
				case SerializedPropertyType.FixedBufferSize:
					throw new InvalidOperationException("FixedBufferSize value cannot be set");
				case SerializedPropertyType.Float:
					property.floatValue = 0;
					break;
				case SerializedPropertyType.Gradient:
					property.SetGradientValue(default);
					break;
				case SerializedPropertyType.Integer: 
				case SerializedPropertyType.LayerMask: 
					property.intValue = 0;
					break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = null;
                    break;
				case SerializedPropertyType.Quaternion: 
					property.quaternionValue = default;
					break;
				case SerializedPropertyType.Rect: 
					property.rectValue = default;
					break;
				case SerializedPropertyType.RectInt: 
					property.rectIntValue = default;
					break;
				case SerializedPropertyType.String:
					property.stringValue = "";
					break;
				case SerializedPropertyType.Vector2: 
					property.vector2Value = default;
					break;
				case SerializedPropertyType.Vector2Int: 
					property.vector2IntValue = default;
					break;
				case SerializedPropertyType.Vector3: 
					property.vector3Value = default;
					break;
				case SerializedPropertyType.Vector3Int: 
					property.vector3IntValue = default;
					break;
				case SerializedPropertyType.Vector4: 
					property.vector4Value = default;
					break;
				case SerializedPropertyType.Generic:
					SetDefaultConstructorValue(property);
					break;
				case SerializedPropertyType.ObjectReference:
				case SerializedPropertyType.AnimationCurve:
				default:
					property.objectReferenceValue = null;
					break;
			}
        }

		public static T GetAttribute<T>(this SerializedProperty property) where T : PropertyAttribute
		{
			var info = GetObject(property, true).GetType().GetField(property.name, InstanceAny);
			return info.GetCustomAttribute<T>();
		}

		public static IEnumerable<T> GetAttributes<T>(this SerializedProperty property) where T : PropertyAttribute
		{
			var info = GetObject(property, true).GetType().GetField(property.name, InstanceAny);
			return info.GetCustomAttributes<T>();
		}
    }
}
