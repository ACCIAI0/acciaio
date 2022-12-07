using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acciaio.Editor
{
	[InitializeOnLoad]
	public sealed class EditorScenesSettings : ScriptableObject
	{
		/// <summary>
		/// Editor Prefs key for retrieving the scene that is currently being edited.
		/// </summary>
		private const string EDITING_SCENE_PREFS_KEY = "Acciaio.Editor.EditingScene";

		public const string SETTINGS_PATH = "Assets/Editor/EditorScenesSettings.asset";

        static EditorScenesSettings() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

		private static void SetPlayModeStartScene(string scene)
		{
			SceneAsset asset = null;
			if (!string.IsNullOrEmpty(scene))
			{
				var path = EditorBuildSettings.scenes.FirstOrDefault(s => s.path.EndsWith($"{scene}.unity"))?.path;
				if (path == null)
					Debug.LogError($"Scene {scene} is not present in build settings.");
				else asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
			}

			EditorSceneManager.playModeStartScene = asset;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange change)
		{
			var settings = GetOrCreateSettings();
			if (change == PlayModeStateChange.ExitingEditMode)
			{
				SetPlayModeStartScene(settings != null && settings._isActive ? settings._startupScene : null);

				if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorApplication.isPlaying = false;
					return;
				}
				EditorPrefs.SetString(EDITING_SCENE_PREFS_KEY, SceneManager.GetActiveScene().name);
			}
			else if (change == PlayModeStateChange.EnteredEditMode) EditorPrefs.DeleteKey(EDITING_SCENE_PREFS_KEY);
		}

		internal static EditorScenesSettings GetOrCreateSettings()
		{
            static bool BuildPathIfNecessary()
			{
				var parent = SETTINGS_PATH[..SETTINGS_PATH.LastIndexOf("/")];
				if (AssetDatabase.IsValidFolder(parent))
					return true;
				var folders = parent.Split('/', System.StringSplitOptions.RemoveEmptyEntries);

				if (folders.Length == 0 || folders[0] != "Assets")
				{
					Debug.LogError("Path must start with \"Assets\".");
					return false;
				}

				string path = folders[0];
				for (int i = 1; i < folders.Length; i++)
				{
					if (!AssetDatabase.IsValidFolder($"{path}/{folders[i]}"))
						AssetDatabase.CreateFolder(path, folders[i]);
					path += $"/{folders[i]}";
				}
				return true;
			}

			var settings = AssetDatabase.LoadAssetAtPath<EditorScenesSettings>(SETTINGS_PATH);
			if (settings == null)
			{
				if (!BuildPathIfNecessary()) return null;
				settings = CreateInstance<EditorScenesSettings>();
				AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		internal static SerializedObject GetSerializedSettings() => new(GetOrCreateSettings());

		[SerializeField]
		private bool _isActive = false;

		[SerializeField]
		private string _startupScene = null;
	}
}