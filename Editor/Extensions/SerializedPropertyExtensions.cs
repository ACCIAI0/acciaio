using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Acciaio.Editor
{
    public static class SerializedPropertyExtensions
    {
        private const string PATH_ELEMENT_ARRAY = "Array";

        private static object GetObject(SerializedProperty property, bool stopAtParent)
        {
            SerializedObject sObj = property.serializedObject;
            sObj.ApplyModifiedProperties();
            object @object = sObj.targetObject;

            string[] elements = property.propertyPath.Split('.');
            for (int i = 0; i < elements.Length - Convert.ToInt32(stopAtParent); i++)
            {
                string pathElement = elements[i];

                if (pathElement == PATH_ELEMENT_ARRAY)
                {
                    int index = int.Parse(elements[i + 1].Replace("data[", "").Replace("]", ""));
                    int j = -1;
                    IEnumerator enumerator = (@object as IEnumerable).GetEnumerator();
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
                    @object = @object.GetType()
                        .GetField(pathElement, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(@object);
                }
            }
            return @object;
        }

        public static Gradient GetGradientValue(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Gradient) return null;
            return property.GetType()
                    .GetProperty("gradientValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
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
                    .GetProperty("gradientValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(property, gradient);
        }

        public static T GetValue<T>(this SerializedProperty property)
        {
            object @object = GetObject(property, false);
            if (@object.GetType() == typeof(T)) return (T)@object;
            Debug.LogError($"{property.name} is not a valid {typeof(T).Name}");
            return default(T);
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
					property.boundsValue = default(Bounds);
					break;
				case SerializedPropertyType.BoundsInt: 
					property.boundsIntValue = default(BoundsInt);
					break;
				case SerializedPropertyType.Character: 
					property.stringValue = "" + default(char);
					break;
				case SerializedPropertyType.Color: 
					property.colorValue = default(Color);
					break;
				case SerializedPropertyType.Enum: 
					property.enumValueIndex = 0;
					break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = null;
                    break;
				case SerializedPropertyType.FixedBufferSize:
					throw new System.InvalidOperationException("FixedBufferSize value cannot be set");
				case SerializedPropertyType.Float:
					property.floatValue = 0;
					break;
				case SerializedPropertyType.Gradient:
					property.SetGradientValue(default(Gradient));
					break;
				case SerializedPropertyType.Integer: 
				case SerializedPropertyType.LayerMask: 
					property.intValue = 0;
					break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = null;
                    break;
				case SerializedPropertyType.Quaternion: 
					property.quaternionValue = default(Quaternion);
					break;
				case SerializedPropertyType.Rect: 
					property.rectValue = default(Rect);
					break;
				case SerializedPropertyType.RectInt: 
					property.rectIntValue = default(RectInt);
					break;
				case SerializedPropertyType.String:
					property.stringValue = "";
					break;
				case SerializedPropertyType.Vector2: 
					property.vector2Value = default(Vector2);
					break;
				case SerializedPropertyType.Vector2Int: 
					property.vector2IntValue = default(Vector2Int);
					break;
				case SerializedPropertyType.Vector3: 
					property.vector3Value = default(Vector3);
					break;
				case SerializedPropertyType.Vector3Int: 
					property.vector3IntValue = default(Vector3Int);
					break;
				case SerializedPropertyType.Vector4: 
					property.vector4Value = default(Vector4);
					break;
				default:
					property.objectReferenceValue = null;
					break;
			}
        }
    }
}
