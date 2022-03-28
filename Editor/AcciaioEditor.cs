using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Acciaio.Editor
{
	public static class AcciaioEditor
	{
		private static string[] BuildScenesListForField(bool allowEmpty, string emptyLabel)
		{
			var scenesList = Enumerable.Empty<string>();
			if (allowEmpty) scenesList = Enumerable.Repeat(emptyLabel ?? "-", 1);
			return scenesList.Concat(GetBuildSettingsScenes()).ToArray();
		}

		private static string[] GetBuildSettingsScenesArray(bool includeEnabled = false) => 
			GetBuildSettingsScenes(includeEnabled).ToArray();

		public static IEnumerable<string> GetBuildSettingsScenes(bool includeEnabled = false) 
		{
			return EditorBuildSettings.scenes
				.Where(s => s.enabled || includeEnabled)
				.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path));
		}

		public static string SceneField(Rect rect, string label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesListForField(allowEmpty, emptyLabel);
			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes);
			return scenes[newIndex];
		}

		public static int SceneField(Rect rect, string label, int value)
		{
			var scenes = GetBuildSettingsScenesArray();
			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes);
			return newIndex;
		}

		public static string SceneField(Rect rect, GUIContent label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesListForField(allowEmpty, emptyLabel);
			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes.Select(s => new GUIContent(s)).ToArray());
			return scenes[newIndex];
		}

		public static int SceneField(Rect rect, GUIContent label, int value)
		{
			var scenes = GetBuildSettingsScenesArray();
			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes.Select(s => new GUIContent(s)).ToArray());
			return newIndex;
		}

		public static string SceneField(string label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesListForField(allowEmpty, emptyLabel);

			if (scenes.Length == 0) return "";

			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUILayout.Popup(label, index, scenes);
			return scenes[newIndex];
		}

		public static int SceneField(string label, int value)
		{
			var scenes = GetBuildSettingsScenesArray();
			var index = Mathf.Max(0, System.Array.IndexOf(scenes, value));
			var newIndex = EditorGUILayout.Popup(label, index, scenes.ToArray());
			return newIndex;
		}
	}
}