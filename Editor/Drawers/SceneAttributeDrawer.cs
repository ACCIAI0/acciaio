using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace Acciaio.Editor
{
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
				(!AcciaioEditor.GetBuildSettingsScenes().Any() ? 2 : 1) * EditorGUI.GetPropertyHeight(property.propertyType, label);

		public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (!AcciaioEditor.GetBuildSettingsScenes().Any())
			{
				EditorGUI.HelpBox(rect, "There are no applicable scenes in build order.", MessageType.Info);
				return;
			}

			var obj = property.serializedObject.targetObject;
			var field = obj.GetType()
					.GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var att = field.GetCustomAttribute<SceneAttribute>();

			if (property.propertyType != SerializedPropertyType.String && 
				property.propertyType != SerializedPropertyType.Integer && 
				field.FieldType != typeof(DecoupledScene))
			{
				base.OnGUI(rect, property, label);
				return;
			}

			if (property.propertyType == SerializedPropertyType.Integer)
				property.intValue = AcciaioEditor.SceneField(rect, label, property.intValue);
			else
			{
				var sceneProperty = property.FindPropertyRelative("_scene");
				var value = property.propertyType == SerializedPropertyType.String ? property.stringValue :
						sceneProperty.stringValue;
				var newValue = AcciaioEditor.SceneField(rect, label, value, att.ShowNoneOption, att.NoneOptionLabel);
				if (value == newValue) return;
				value = att.ShowNoneOption && newValue == att.NoneOptionLabel ? null : newValue;
				if (property.propertyType == SerializedPropertyType.String) 
					property.stringValue = value;
				else sceneProperty.stringValue = value;
			}
		}		
	}
}