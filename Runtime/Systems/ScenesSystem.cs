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

namespace Acciaio
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

        [Header("Scenes System")]
		[SerializeField, Scene("-")]
		private string _defaultLoadingScene = "";


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

			if (string.IsNullOrEmpty(loadingScene)) loadingScene = _defaultLoadingScene;

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

			Scene loading = !string.IsNullOrEmpty(loadingScene) ? SceneManager.GetSceneByName(loadingScene) : new();
			if (loading.IsValid())
			{
				SceneManager.SetActiveScene(loading);
				yield return null;
				_currentLoadingView ??= loading.GetRootGameObjects()
                        .Select(go => go.GetComponent<ILoadingView>())
                        .FirstOrDefault(v => v != null);
			}

			yield return _currentLoadingView?.Show();

#if USE_ADDRESSABLES
			if (toUnload == _loadedAddressablesScene.Scene) 
			{
				yield return Addressables.UnloadSceneAsync(_loadedAddressablesScene);
				_loadedAddressablesScene = default;
			}
			else
#endif 
				yield return SceneManager.UnloadSceneAsync(toUnload.name);

#if USE_ADDRESSABLES
			if (useAddressables)
			{
				var handle = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
				yield return handle;
				SceneManager.SetActiveScene(handle.Result.Scene);
				_loadedAddressablesScene = handle.Result;
			}
			else
			{
#endif
				yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
#if USE_ADDRESSABLES
			}
#endif

			if (autoHideLoadingView)
			{
				IEnumerator hideLoadingRoutine = HideLoadingViewCo();
				while (hideLoadingRoutine.MoveNext()) yield return hideLoadingRoutine.Current;
			}
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

		public SceneOperation LoadScene(string scene, bool useAddressables, bool autoHideLoadingView = true) 
			=> LoadScene(scene, useAddressables, null, autoHideLoadingView);

		public SceneOperation LoadScene(string scene, bool useAddressables, string loadingScene, bool autoHideLoadingView = true)
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

		public SceneOperation AddScene(string scene) => AddScene(scene, false);

#if USE_ADDRESSABLES
		public SceneOperation AddAddressablesScene(string scenePath) => AddScene(scenePath, true);
#endif

		public SceneOperation AddScene(string scene, bool useAddressables)
		{
			if (!IsRunning)
			{
				Debug.LogError("[Scenes System] System shut down, cannot add a scene.");
				return null;
			}

			return new(this, AddSceneCo(scene, useAddressables));
		}

		public void RemoveScene(string sceneName)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			Scene active = SceneManager.GetActiveScene();

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

		public Scene GetSceneByName(string name) => SceneManager.GetSceneByName(name);

		public YieldInstruction HideCurrentLoadingView() => StartCoroutine(HideLoadingViewCo());
    }
}