using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acciaio.Editor
{
	[CreateAssetMenu(fileName="EditorScenesSettings", menuName="Acciaio/Editor Scenes Settings")]
	[InitializeOnLoad]
	public class EditorScenesSettings : ScriptableObject
	{
		/// <summary>
		/// Editor Prefs key for retrieving the scene that is currently being edited.
		/// </summary>
		private const string EDITING_SCENE_PREFS_KEY = "Acciaio.Editor.EditingScene";

		private static EditorScenesSettings _instance;

		private static EditorScenesSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					var assets = AssetDatabase.FindAssets($"t:{nameof(EditorScenesSettings)}");
					if (assets.Length == 0) return null;
					if (assets.Length > 1)
						Debug.LogWarning("Multiple Editor Scenes Setups found, only one will be used.");
					_instance = AssetDatabase.LoadAssetAtPath<EditorScenesSettings>(AssetDatabase.GUIDToAssetPath(assets[0]));
				}
				return _instance;
			}
		}

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
			if (change == PlayModeStateChange.ExitingEditMode)
			{
				SetPlayModeStartScene(Instance != null && Instance._isActive ? Instance._startupScene : null);

				if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorApplication.isPlaying = false;
					return;
				}
				EditorPrefs.SetString(EDITING_SCENE_PREFS_KEY, SceneManager.GetActiveScene().name);
			}
			else if (change == PlayModeStateChange.EnteredEditMode) EditorPrefs.DeleteKey(EDITING_SCENE_PREFS_KEY);
		}

		[SerializeField]
		private bool _isActive = false;

		[SerializeField, Scene]
		private string _startupScene = null;
	}
}