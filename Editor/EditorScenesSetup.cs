using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Acciaio.Editor
{
	[CreateAssetMenu(fileName="EditorScenesSetup", menuName="Silver Lomami/Editor Scenes Setup")]
	[InitializeOnLoad]
	public class EditorScenesSetup : ScriptableObject
	{
		/// <summary>
		/// Editor Prefs key for retrieving the scene that is currently being edited.
		/// </summary>
		public const string EDITING_SCENE_PREFS_KEY = "Acciaio.Editor.EditingScene";

		/// <summary>
		/// Editor Prefs key for retrieving the the scene to play at the start for local override.
		/// </summary>
		public const string LOCAL_START_OVERRIDE_KEY = "Acciaio.Editor.StartOverrideScene";

		private static EditorScenesSetup _instance;

		private static EditorScenesSetup Instance
		{
			get
			{
				if (_instance == null)
				{
					string[] assets = AssetDatabase.FindAssets($"t:{nameof(EditorScenesSetup)}");
					if (assets.Length == 0) return null;
					if (assets.Length > 1)
						Debug.LogWarning("Multiple Editor Scenes Setups found, only one will be used.");
					_instance = AssetDatabase.LoadAssetAtPath<EditorScenesSetup>(AssetDatabase.GUIDToAssetPath(assets[0]));
				}
				return _instance;
			}
		}

        static EditorScenesSetup() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

		private static void SetPlayModeStartScene(string scene)
		{
			SceneAsset asset = null;
			if (!string.IsNullOrEmpty(scene))
			{
				string path = EditorBuildSettings.scenes.FirstOrDefault(s => s.path.EndsWith($"{scene}.unity"))?.path;
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
				string scene = null;
				if (Instance != null) 
					scene = EditorPrefs.GetString(LOCAL_START_OVERRIDE_KEY, null) ?? Instance._startupScene;
				SetPlayModeStartScene(scene);

				if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorApplication.isPlaying = false;
					return;
				}
				EditorPrefs.SetString(EDITING_SCENE_PREFS_KEY, EditorSceneManager.GetActiveScene().name);
			}
			else if (change == PlayModeStateChange.EnteredEditMode) EditorPrefs.DeleteKey(EDITING_SCENE_PREFS_KEY);
		}

		[SerializeField, Scene]
		private string _startupScene = null;
	}
}