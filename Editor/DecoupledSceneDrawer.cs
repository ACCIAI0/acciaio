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
			SerializedProperty scene = property.FindPropertyRelative("_scene");

			bool isPresent = EditorBuildSettings.scenes
					.Where(s => s.enabled)
					.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
					.Any(name => scene.stringValue == name);

			GUIStyle style = new GUIStyle(EditorStyles.textField);
			Color defaultNormal = EditorStyles.label.normal.textColor;
			Color defaultHover = EditorStyles.label.hover.textColor;
			Color defaultFocus = EditorStyles.label.focused.textColor;
			if (!isPresent) 
			{
				EditorStyles.label.normal.textColor = Color.yellow;
				EditorStyles.label.hover.textColor = new Color(1, .55f, 0, 1);
				EditorStyles.label.focused.textColor = new Color(.5f, 0, .5f, 1);
				label.tooltip = $"'{scene.stringValue}' is not in the build order";
			}

			Rect singleRect = new Rect(rect);
			singleRect.height = EditorGUIUtility.singleLineHeight;
			scene.stringValue = EditorGUI.TextField(singleRect, label, scene.stringValue);

			EditorStyles.label.normal.textColor = defaultNormal;
			EditorStyles.label.hover.textColor = defaultHover;
			EditorStyles.label.focused.textColor = defaultFocus;
		}
	}
}