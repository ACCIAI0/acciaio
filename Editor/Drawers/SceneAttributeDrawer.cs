using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

namespace Acciaio.Editor
{
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public sealed class SceneAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
				(!AcciaioEditor.GetBuildSettingsScenes().Any() ? 2 : 1) * EditorGUI.GetPropertyHeight(property.propertyType, label);

		public void OnGUI(Rect rect, SerializedProperty property, GUIContent label, bool allowEmpty, string emptyLabel)
		{
			if (property.propertyType != SerializedPropertyType.String && 
				property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.PropertyField(rect, property, label);
				return;
			}

			if (property.propertyType == SerializedPropertyType.Integer)
				property.intValue = AcciaioEditor.SceneField(rect, label, property.intValue);
			else
			{
				var sceneProperty = property.FindPropertyRelative("_scene");
				var value = property.propertyType == SerializedPropertyType.String ? property.stringValue :
						sceneProperty.stringValue;
				var newValue = AcciaioEditor.SceneField(rect, label, value, allowEmpty, emptyLabel);
				if (value == newValue) return;
				if (property.propertyType == SerializedPropertyType.String) 
					property.stringValue = newValue;
				else sceneProperty.stringValue = newValue;
			}
		}

		public VisualElement CreatePropertyGUI(SerializedProperty property, bool allowEmpty, string emptyLabel)
		{
			if (property.propertyType != SerializedPropertyType.String && property.propertyType != SerializedPropertyType.Integer)
				return new PropertyField(property);

			VisualElement element = null;
			if (property.propertyType == SerializedPropertyType.Integer)
			{
				element = AcciaioEditor.CreateSceneField(
					ObjectNames.NicifyVariableName(property.name), 
					property.intValue,
					i => 
					{
						property.intValue = i;
						property.serializedObject.ApplyModifiedProperties();
					});
			}
			else
			{
				element = AcciaioEditor.CreateSceneField(
					ObjectNames.NicifyVariableName(property.name),
					property.stringValue, 
					allowEmpty, 
					emptyLabel,
					s => 
					{
						property.stringValue = s;
						property.serializedObject.ApplyModifiedProperties();
					});
			}
			return element;
		}

		public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (!AcciaioEditor.GetBuildSettingsScenes().Any())
			{
				EditorGUI.HelpBox(rect, "There are no applicable scenes in build order.", MessageType.Info);
				return;
			}
			var att = fieldInfo.GetCustomAttribute<SceneAttribute>();
			OnGUI(rect, property, label, att.ShowNoneOption, att.NoneOptionLabel);
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (!AcciaioEditor.GetBuildSettingsScenes().Any())
				return new HelpBox("There are no applicable scenes in build order.", HelpBoxMessageType.Info);

			var att = fieldInfo.GetCustomAttribute<SceneAttribute>();
			return CreatePropertyGUI(property, att.ShowNoneOption, att.NoneOptionLabel);
		}	
	}
}