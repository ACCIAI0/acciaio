using UnityEditor;
using UnityEngine;

namespace Acciaio.Editor
{
	[CustomEditor(typeof(EditorScenesSettings))]
	public class EditorScenesSettingsEditor : UnityEditor.Editor
	{
		private SerializedProperty _startupScene;
		private SerializedProperty _isActive;

		private void OnEnable()
		{
			_startupScene = serializedObject.FindProperty("_startupScene");
			_isActive = serializedObject.FindProperty("_isActive");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(!_isActive.boolValue);
			_startupScene.stringValue = AcciaioEditor.SceneField("Startup Scene", _startupScene.stringValue, true, "(None)");
			EditorGUI.EndDisabledGroup();
			_isActive.boolValue = EditorGUILayout.Toggle(GUIContent.none, _isActive.boolValue, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}	
	}
}