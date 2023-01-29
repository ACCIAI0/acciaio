using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acciaio.Editor
{
    public static class AcciaioMenuItems
    {
        private const string SystemsScenePathFormat = "Assets/_Scenes/{0}.unity";

        private static bool AssetExists<T>(string path) where T : Object 
            => AssetDatabase.LoadAssetAtPath<T>(path) != null;

        [MenuItem("Tools/Acciaio/Create Systems Scene", false, 51)]
        public static void CreateSystemsScene()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Scenes"))
                AssetDatabase.CreateFolder("Assets", "_Scenes");
                
            var systemsScenePath = string.Format(SystemsScenePathFormat, Systems.SceneName);
            if (!AssetExists<SceneAsset>(systemsScenePath))
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.LogWarning("Systems scene was not created");
                    return;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                var go = new GameObject("ScenesSystem", typeof(ScenesSystem));
                SceneManager.MoveGameObjectToScene(go, scene);

                go = new GameObject("PubSubSystem", typeof(PubSubSystem));
                SceneManager.MoveGameObjectToScene(go, scene);

                EditorSceneManager.SaveScene(scene, systemsScenePath);

                Debug.Log("Created Systems scene");
            }

            if (EditorBuildSettings.scenes.All(s => s.path != systemsScenePath))
            {
                var scenes = System.Linq.Enumerable.Repeat(new EditorBuildSettingsScene(systemsScenePath, true), 1)
                        .Concat(EditorBuildSettings.scenes)
                        .ToArray();
                EditorBuildSettings.scenes = scenes;
                Debug.Log("Added Systems scene to build order");
            }
        }
    }
}
