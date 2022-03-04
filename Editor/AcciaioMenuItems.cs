using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Acciaio.Editor
{
    public static class AcciaioMenuItems
    {
        private const string SCENES_SETUP_PATH = "Assets/Editor/EditorScenesSettings.asset";
        private const string SYSTEMS_SCENE_PATH_FORMAT = "Assets/_Scenes/{0}.unity";

        private const string DIALOG_MESSAGE = "This will create and add a new scene in the build order and a settings asset in Assets/Editor, proceed?";

        private static bool AssetExists<T>(string path) where T : Object => 
            AssetDatabase.LoadAssetAtPath<T>(path) != null;

        [MenuItem("Tools/Acciaio/Create Editor Scenes Settings", false, 50)]
        private static void CreateEditorScenesSettings()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            
            if (!AssetExists<EditorScenesSettings>(SCENES_SETUP_PATH))
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EditorScenesSettings>(), SCENES_SETUP_PATH);
                Debug.Log("Created an Editor Scenes Settings asset");
                AssetDatabase.Refresh();
            }

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(SCENES_SETUP_PATH);
        }

        [MenuItem("Tools/Acciaio/Create Systems Scene", false, 51)]
        private static void CreateSystemsScene()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Scenes"))
                AssetDatabase.CreateFolder("Assets", "_Scenes");
                
            string systemsScenePath = string.Format(SYSTEMS_SCENE_PATH_FORMAT, Systems.SCENE_NAME);
            if (!AssetExists<SceneAsset>(systemsScenePath))
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.LogWarning("Systems scene was not created");
                    return;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                var go = new GameObject("ScenesSystem", typeof(ScenesSystem));
                EditorSceneManager.MoveGameObjectToScene(go, scene);

                go = new GameObject("PubSubSystem", typeof(PubSubSystem));
                EditorSceneManager.MoveGameObjectToScene(go, scene);

                EditorSceneManager.SaveScene(scene, systemsScenePath);

                Debug.Log("Created Systems scene");
            }

            if (EditorBuildSettings.scenes.All(s => s.path != systemsScenePath))
            {
                var scenes = Enumerable.Repeat(new EditorBuildSettingsScene(systemsScenePath, true), 1)
                        .Concat(EditorBuildSettings.scenes)
                        .ToArray();
                EditorBuildSettings.scenes = scenes;
                Debug.Log("Added Systems scene to build order");
            }
        }

        [MenuItem("Tools/Acciaio/Full Setup", false, 0)]
        public static void SetupAcciaio()
        {
            if (!EditorUtility.DisplayDialog("Acciaio Auto Setup", DIALOG_MESSAGE, "Proceed", "Cancel")) return;
            CreateSystemsScene();
            CreateEditorScenesSettings();
        }
    }
}
