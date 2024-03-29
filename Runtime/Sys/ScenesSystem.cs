using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if USE_ADDRESSABLES
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace Acciaio.Sys
{
    public sealed class ScenesSystem : BaseSystem<ScenesSystem>
    {
		public sealed class SceneOperation : CustomYieldInstruction
		{
			public event Action<SceneOperation> Completed;

			private readonly ScenesSystem _system;

			private bool _keepWaiting = true;

			public override bool keepWaiting => _keepWaiting;

			internal SceneOperation(ScenesSystem system, IEnumerator routine)
			{
				_system = system;
				_system.StartCoroutine(Internal(routine));
			}

			private IEnumerator Internal(IEnumerator routine)
			{
				while (routine.MoveNext()) yield return routine.Current;
				if (_system._operation == this) _system._operation = null;
				_keepWaiting = false;
				Completed?.Invoke(this);
			}

            public YieldInstruction HideCurrentLoadingView() => _system.HideCurrentLoadingView();
        }
		
		[SerializeField]
		private SceneReference _defaultLoadingScene;


#if USE_ADDRESSABLES
		private readonly Dictionary<string, SceneInstance> _addedAddressablesScenes = new();
		private SceneInstance _loadedAddressablesScene = default;
#endif
		
		private bool _isRunning;
		private SceneOperation _operation;
		private ILoadingView _currentLoadingView;

        public override bool IsRunning => _isRunning;

		public Scene ActiveScene => SceneManager.GetActiveScene();

		private IEnumerator HideLoadingViewCo()
		{
			if (_currentLoadingView == null) yield break;
			yield return _currentLoadingView.Hide();
			yield return SceneManager.UnloadSceneAsync(_currentLoadingView.Scene);
			_currentLoadingView = null;
		}

		private IEnumerator LoadSceneCo(string scene, bool useAddressables, string loadingScene, bool autoHideLoadingView)
		{
			var toUnload = SceneManager.GetActiveScene();

			if (string.IsNullOrEmpty(loadingScene)) loadingScene = _defaultLoadingScene.Path;

			if (!string.IsNullOrEmpty(loadingScene))
			{
				if (_currentLoadingView != null && _currentLoadingView.Scene.name.Equals(loadingScene, StringComparison.Ordinal))
				{
					SceneManager.UnloadSceneAsync(_currentLoadingView.Scene);
					_currentLoadingView = null;
				}
				if (_currentLoadingView == null) 
					yield return SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
			}

			var loading = !string.IsNullOrEmpty(loadingScene) ? GetSceneByPathOrName(loadingScene) : new();
			if (loading.IsValid())
			{
				SceneManager.SetActiveScene(loading);
				yield return null;
				_currentLoadingView ??= loading.GetRootGameObjects()
                        .Select(go => go.GetComponent<ILoadingView>())
                        .FirstOrDefault(v => v != null);
			}

			yield return _currentLoadingView?.Show();

            var progressLoadingView = _currentLoadingView as IProgressLoadingView;

#if USE_ADDRESSABLES
			if (toUnload == _loadedAddressablesScene.Scene) 
			{
				var unloadOperation = Addressables.UnloadSceneAsync(_loadedAddressablesScene);
                if (progressLoadingView == null) yield return unloadOperation;
                else
                {
                    while (!unloadOperation.IsDone)
                    {
                        progressLoadingView.CurrentProgress = unloadOperation.PercentComplete / 2;
                        yield return null;
                    }
                }
				_loadedAddressablesScene = default;
			}
			else
            {
#endif 
				var unloadOperation = SceneManager.UnloadSceneAsync(toUnload.name);
                if (progressLoadingView == null) yield return unloadOperation;
                else
                {
                    while (!unloadOperation.isDone)
                    {
                        progressLoadingView.CurrentProgress = unloadOperation.progress / 2;
                        yield return null;
                    }
                }
#if USE_ADDRESSABLES
			}
#endif

#if USE_ADDRESSABLES
			if (useAddressables)
			{
				var loadOperation = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
                if (progressLoadingView == null) yield return loadOperation;
                else
                {
                    while (!loadOperation.IsDone)
                    {
                        progressLoadingView.CurrentProgress = .5f + loadOperation.PercentComplete / 2;
                        yield return null;
                    }
                }
				SceneManager.SetActiveScene(loadOperation.Result.Scene);
				_loadedAddressablesScene = loadOperation.Result;
			}
			else
			{
#endif
				var loadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                if (progressLoadingView == null) yield return loadOperation;
                else
                {
                    while (!loadOperation.isDone)
                    {
                        progressLoadingView.CurrentProgress = .5f + loadOperation.progress / 2;
                        yield return null;
                    }
                }
                
				SceneManager.SetActiveScene(GetSceneByPathOrName(scene));
#if USE_ADDRESSABLES
			}
#endif

			if (!autoHideLoadingView) yield break;
            
			var hideLoadingRoutine = HideLoadingViewCo();
			while (hideLoadingRoutine.MoveNext()) yield return hideLoadingRoutine.Current;
		}

		private IEnumerator AddSceneCo(string scene, bool useAddressables)
		{
#if USE_ADDRESSABLES
			if (useAddressables)
			{
				var handle = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
				yield return handle;
				if (handle.Status == AsyncOperationStatus.Succeeded) 
					_addedAddressablesScenes.Add(scene, handle.Result);
				else Debug.LogError($"Failed to load addressables scene '{scene}'.");
			}
			else 
#endif
				yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		}

		private SceneOperation LoadScene(string scene, bool useAddressables, string loadingScene, bool autoHideLoadingView = true)
		{
			if (!IsRunning)
			{
				Debug.LogError("[Scenes System] System shut down, cannot load a scene.");
				return null;
			}

			if (_operation != null)
				throw new InvalidOperationException("A scene is already being loaded, cannot load another one");

			_operation = new(this, LoadSceneCo(scene, useAddressables, loadingScene, autoHideLoadingView));
			return _operation;
		}

		private SceneOperation AddScene(string scene, bool useAddressables)
		{
			if (!IsRunning)
			{
				Debug.LogError("[Scenes System] System shut down, cannot add a scene.");
				return null;
			}

			return new(this, AddSceneCo(scene, useAddressables));
		}

        protected override IEnumerator RunRoutine()
        {
            Debug.Log("[Scenes System] Online");
			_isRunning = true;
			yield break;
        }

        protected override IEnumerator ShutdownRoutine()
        {
            Debug.Log("[Scenes System] Shutdown");
			_isRunning = false;
			yield break;
        }

		public SceneOperation LoadScene(string scene, bool autoHideLoadingView = true) 
			=> LoadScene(scene, null, autoHideLoadingView);

		public SceneOperation LoadScene(string scene, string loadingScene, bool autoHideLoadingView = true) 
			=> LoadScene(scene, false, loadingScene, autoHideLoadingView);

#if USE_ADDRESSABLES
		public SceneOperation LoadAddressableScene(string scenePath, bool autoHideLoadingView = true) 
			=> LoadAddressableScene(scenePath, null, autoHideLoadingView);

		public SceneOperation LoadAddressableScene(string scenePath, string loadingScene, bool autoHideLoadingView = true) 
			=> LoadScene(scenePath, true, loadingScene, autoHideLoadingView);
#endif

		public SceneOperation LoadScene(string scene, bool useAddressables, bool autoHideLoadingView) 
			=> LoadScene(scene, useAddressables, null, autoHideLoadingView);

		public SceneOperation LoadScene(SceneReference scene, bool autoHideLoadingView = true)
			=> LoadScene(scene, null, autoHideLoadingView);

		public SceneOperation LoadScene(SceneReference scene, SceneReference loadingScene, bool autoHideLoadingView = true)
		{
			var path = scene.Path;
			var isAddressable = false;
			
#if USE_ADDRESSABLES
			isAddressable = scene.IsAddressable;
#endif
			
#if UNITY_EDITOR
			if (scene.IsEditorOverrideable)
			{
				var editorScene = UnityEditor.EditorPrefs.GetString("Acciaio.Editor.EditingScene", null);
				if (!string.IsNullOrEmpty(editorScene) && SceneManager.GetActiveScene().name != editorScene)
					path = editorScene;
			}
#endif
			return LoadScene(path, isAddressable, loadingScene?.Path, autoHideLoadingView);
		}

		public SceneOperation AddScene(string scene) => AddScene(scene, false);

#if USE_ADDRESSABLES
		public SceneOperation AddAddressablesScene(string scenePath) => AddScene(scenePath, true);
#endif

	    public SceneOperation AddScene(SceneReference scene)
	    {
		    var path = scene.Path;
		    var isAddressable = false;
#if USE_ADDRESSABLES
		    isAddressable = scene.IsAddressable;
#endif
		    return AddScene(path, isAddressable);
	    }

		public void RemoveScene(string sceneName)
		{
			var scene = SceneManager.GetSceneByName(sceneName);
			var active = SceneManager.GetActiveScene();

			if (scene == active) Debug.LogWarning($"Removing active scene {sceneName}.");

#if USE_ADDRESSABLES
			if (scene == _loadedAddressablesScene.Scene)
			{
				Addressables.UnloadSceneAsync(_loadedAddressablesScene);
				_loadedAddressablesScene = default;
				return;
			}
			
			if (_addedAddressablesScenes.TryGetValue(sceneName, out var instance))
			{
				Addressables.UnloadSceneAsync(instance);
				_addedAddressablesScenes.Remove(sceneName);
				return;
			}
#endif

			SceneManager.UnloadSceneAsync(scene);
		}

		public Scene GetSceneByName(string sceneName) => SceneManager.GetSceneByName(sceneName);

		public Scene GetSceneByPath(string scenePath) => SceneManager.GetSceneByPath(scenePath);

		public Scene GetSceneByPathOrName(string pathOrName)
		{
			if (pathOrName.Contains("/") || pathOrName.EndsWith(".unity")) return GetSceneByPath(pathOrName);
			return GetSceneByName(pathOrName);
		}
		
		public YieldInstruction HideCurrentLoadingView() => StartCoroutine(HideLoadingViewCo());
    }
}