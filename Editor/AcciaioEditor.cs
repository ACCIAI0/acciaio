using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
	public static class AcciaioEditor
	{
		public static string[] BuildScenesArrayForField(bool allowEmpty, string emptyLabel)
		{
			var scenesList = System.Linq.Enumerable.Empty<string>();
			if (allowEmpty) scenesList = System.Linq.Enumerable.Repeat(emptyLabel ?? "-", 1);
			return scenesList.Concat(GetBuildSettingsScenes()).ToArray();
		}

		public static IEnumerable<string> GetBuildSettingsScenes(bool includeEnabled = false) 
		{
			return EditorBuildSettings.scenes
                    .Where(s => s.enabled || includeEnabled)
                    .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path));
		}

		public static string SceneField(Rect rect, string label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesArrayForField(allowEmpty, emptyLabel);
			var index = Mathf.Max(0, Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes);
			return allowEmpty && scenes[newIndex] == emptyLabel ? "" : scenes[newIndex];
		}

		public static int SceneField(Rect rect, string label, int value)
		{
			var scenes = BuildScenesArrayForField(false, null);
			var index = Mathf.Clamp(value, 0, scenes.Length);
			var newIndex = EditorGUI.Popup(rect, label, index, scenes);
			return newIndex;
		}

		public static string SceneField(Rect rect, GUIContent label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesArrayForField(allowEmpty, emptyLabel);
			var index = Mathf.Max(0, Array.IndexOf(scenes, value));
			var newIndex = EditorGUI.Popup(rect, label, index, scenes.Select(s => new GUIContent(s)).ToArray());
			return allowEmpty && scenes[newIndex] == emptyLabel ? "" : scenes[newIndex];
		}

		public static int SceneField(Rect rect, GUIContent label, int value)
		{
			var scenes = BuildScenesArrayForField(false, null);
			var index = Mathf.Clamp(value, 0, scenes.Length);
			var newIndex = EditorGUI.Popup(rect, label, index, scenes.Select(s => new GUIContent(s)).ToArray());
			return newIndex;
		}

		public static string SceneField(string label, string value, bool allowEmpty, string emptyLabel)
		{
			var scenes = BuildScenesArrayForField(allowEmpty, emptyLabel);

			if (scenes.Length == 0) return "";

			var index = Mathf.Max(0, Array.IndexOf(scenes, value));
			var newIndex = EditorGUILayout.Popup(label, index, scenes);
			return allowEmpty && scenes[newIndex] == emptyLabel ? "" : scenes[newIndex];
		}

		public static int SceneField(string label, int value)
		{
			var scenes = BuildScenesArrayForField(false, null);
			var index = Mathf.Clamp(value, 0, scenes.Length);
			var newIndex = EditorGUILayout.Popup(label, index, scenes.ToArray());
			return newIndex;
		}

		public static VisualElement CreateSceneField(
            string label, 
            string value, 
            bool allowEmpty, 
            string emptyLabel, 
            Action<string> onValueChanged)
		{
			string Display(string s) => allowEmpty && string.IsNullOrEmpty(s) ? emptyLabel : s;

			var scenes = BuildScenesArrayForField(allowEmpty, "").ToList();
			PopupField<string> popup = new(label, scenes, value, Display, Display);
			popup.RegisterCallback<ChangeEvent<string>>(evt => onValueChanged?.Invoke(evt.newValue));
			return popup;
		}

		public static VisualElement CreateSceneField(string label, int value, Action<int> onValueChanged)
		{
			var scenes = BuildScenesArrayForField(false, null).ToList();
			PopupField<string> popup = new(label, scenes, value);
			popup.RegisterCallback<ChangeEvent<string>>(evt => onValueChanged?.Invoke(scenes.IndexOf(evt.newValue)));
			return popup;
		}
	}
}