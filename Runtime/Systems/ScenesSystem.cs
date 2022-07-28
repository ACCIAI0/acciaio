using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace Acciaio
{
    public class ScenesSystem : BaseSystem<ScenesSystem>
    {
		public sealed class SceneOperation : CustomYieldInstruction
		{
			private static SceneOperation Create(ScenesSystem system, IEnumerator routine) => new(system, routine);

			public static void PrepareType() => createOperation = Create;

			public event Action<SceneOperation> Completed;

			private readonly ScenesSystem _system;

			private bool _keepWaiting = true;

			public override bool keepWaiting => _keepWaiting;

			private SceneOperation(ScenesSystem system, IEnumerator routine)
			{
				_system = system;
				_system.StartCoroutine(Internal(routine));
			}

			private IEnumerator Internal(IEnumerator routine)
			{
				yield return _system.StartCoroutine(routine);
				if (_system._operation == this) _system._operation = null;
				_keepWaiting = false;
				Completed?.Invoke(this);
			}
		}

		private static Func<ScenesSystem, IEnumerator, SceneOperation> createOperation = default;

        static ScenesSystem() => SceneOperation.PrepareType();

        [Header("Scenes System")]
		[SerializeField, Scene("-")]
		private string _defaultLoadingScene = "";


#if USE_ADDRESSABLES
		private readonly Dictionary<string, SceneInstance> _addedAddressablesScenes = new();
		private SceneInstance _loadedAddressablesScene = default;
#endif
		
		private bool _isRunning;
		private SceneOperation _operation;

        public override bool IsRunning => _isRunning;

		private IEnumerator LoadSceneCo(string scene, bool useAddressables, string loadingScene)
		{
			var toUnload = SceneManager.GetActiveScene();

			if (string.IsNullOrEmpty(loadingScene)) loadingScene = _defaultLoadingScene;

			if (!string.IsNullOrEmpty(loadingScene))
				yield return SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);

			Scene loading = !string.IsNullOrEmpty(loadingScene) ? SceneManager.GetSceneByName(loadingScene) : new Scene();
			ILoadingView view = null;
			if (loading.IsValid())
			{
				SceneManager.SetActiveScene(loading);
				yield return null;
				view = loading.GetRootGameObjects()
					.Select(go => go.GetComponent<ILoadingView>())
					.FirstOrDefault(v => v != null);
			}

			yield return view?.Show();

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

			if (loading.IsValid())
			{
				yield return view?.Hide();
				SceneManager.UnloadSceneAsync(loadingScene);
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

		public SceneOperation LoadScene(string scene) => LoadScene(scene, null);

		public SceneOperation LoadScene(string scene, string loadingScene) => LoadScene(scene, false, loadingScene);

#if USE_ADDRESSABLES
		public SceneOperation LoadAddressableScene(string scenePath) => LoadAddressableScene(scenePath, null);

		public SceneOperation LoadAddressableScene(string scenePath, string loadingScene) => LoadScene(scenePath, true, loadingScene);
#endif

		public SceneOperation LoadScene(string scene, bool useAddressables) => LoadScene(scene, useAddressables, null);

		public SceneOperation LoadScene(string scene, bool useAddressables, string loadingScene)
		{
			if (!IsRunning)
			{
				Debug.LogError("[Scenes System] System shut down, cannot load a scene.");
				return null;
			}

			if (_operation != null)
				throw new InvalidOperationException("A scene is already being loaded, cannot load another one");

			_operation = createOperation(this, LoadSceneCo(scene, useAddressables, loadingScene));
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

			return createOperation(this, AddSceneCo(scene, useAddressables));
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
    }
}