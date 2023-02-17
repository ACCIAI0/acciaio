using System;
using System.Linq;
using Acciaio.Editor.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Acciaio.Editor
{
    public class SceneReferencesConsistencyPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private bool IterateObject(Object obj)
        {
            var isOverride = PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected;

            var serializedObject = new SerializedObject(obj);
            var iterator = serializedObject.GetIterator();
            
            if (iterator == null) return false;

            var anyChanged = false;
            do
            {
                if (isOverride && !iterator.prefabOverride) continue;
                if (!iterator.type.Equals(nameof(SceneReference), StringComparison.Ordinal)) continue;
                    
                anyChanged |= SceneReferenceDrawer.UpdateProperty(iterator);
            } while (iterator.Next(true));
                
            if (anyChanged) serializedObject.ApplyModifiedProperties();
            return anyChanged;
        }

        private bool IterateGameObject(GameObject obj)
        {
            var somethingChanged = obj.GetComponents<MonoBehaviour>()
                    .Aggregate(false, (current, behaviour) => current || IterateObject(behaviour));

            return obj.transform.Cast<Transform>()
                    .Aggregate(somethingChanged, (current, child) => current || IterateGameObject(child.gameObject));
        }
        
        private void IterateScenes()
        {
            var paths = AssetDatabase.FindAssets("t:SceneAsset")
                    .Select(AssetDatabase.GUIDToAssetPath);

            foreach (var scenePath in paths)
            {
                if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scenePath) is not null)
                    continue;
                
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
               
               foreach (var obj in scene.GetRootGameObjects())
                   IterateGameObject(obj);
               
               EditorSceneManager.MarkSceneDirty(scene);
               EditorSceneManager.SaveScene(scene);
               EditorSceneManager.CloseScene(scene, true);
            }
        }

        private void IteratePrefabs()
        {
            var assets = AssetDatabase.FindAssets("t:GameObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>);

            foreach (var asset in assets)
            {
                if (IterateGameObject(asset)) 
                    PrefabUtility.SavePrefabAsset(asset);
            }
        }

        private void IterateScriptableObjects()
        {
            var assets = AssetDatabase.FindAssets("t:ScriptableObject")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>);
            foreach (var asset in assets) IterateObject(asset);
            AssetDatabase.SaveAssets();
        }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorScenesSettings.GetOrCreateSettings().EnableReferencesConsistency) return;
            IterateScenes();
            IteratePrefabs();
            IterateScriptableObjects();
        }
    }
}