using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Acciaio.Editor
{
	[CustomPropertyDrawer(typeof(DecoupledScene))]
	public class DecoupledSceneDrawer : UnityEditor.PropertyDrawer
	{
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
			EditorGUI.GetPropertyHeight(property.propertyType, label);

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			var scene = property.FindPropertyRelative("_scene");

			var isPresent = EditorBuildSettings.scenes
					.Where(s => s.enabled)
					.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
					.Any(name => scene.stringValue == name);

			var style = new GUIStyle(EditorStyles.textField);
			var defaultNormal = EditorStyles.label.normal.textColor;
			var defaultHover = EditorStyles.label.hover.textColor;
			var defaultFocus = EditorStyles.label.focused.textColor;
			if (!isPresent) 
			{
				EditorStyles.label.normal.textColor = Color.yellow;
				EditorStyles.label.hover.textColor = new Color(1, .55f, 0, 1);
				EditorStyles.label.focused.textColor = new Color(.5f, 0, .5f, 1);
				label.tooltip = $"'{scene.stringValue}' is not in the build order";
			}

			var singleRect = new Rect(rect)
			{
				height = EditorGUIUtility.singleLineHeight
			};
			scene.stringValue = EditorGUI.TextField(singleRect, label, scene.stringValue);

			EditorStyles.label.normal.textColor = defaultNormal;
			EditorStyles.label.hover.textColor = defaultHover;
			EditorStyles.label.focused.textColor = defaultFocus;
		}
	}
}