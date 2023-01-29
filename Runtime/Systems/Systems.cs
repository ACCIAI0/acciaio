using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acciaio
{
	public static class Systems
	{
        private class SystemOperation : CustomYieldInstruction
        {
			private readonly AsyncOperation _op;
			private bool _keepWaiting = true;

            public override bool keepWaiting => _keepWaiting;

			public SystemOperation(string systemsSceneName, Action<bool> callback)
			{
				_op = SceneManager.LoadSceneAsync(systemsSceneName, LoadSceneMode.Additive);
				_op.completed += _ =>
				{
					var scene = SceneManager.GetSceneByName(systemsSceneName);
					if (!scene.IsValid())
					{
						Debug.LogError($"Systems scene '{systemsSceneName}' was not loaded correctly. Maybe it wasn't added to the build order?");
						_keepWaiting = false;
						callback?.Invoke(false);
						return;
					}

					ActiveSystemsScene = scene;

					var systems = scene.GetRootGameObjects()
							.Select(@object => @object.GetComponent<ISystem>())
							.Where(system => system != null)
							.OrderBy(system => system.Priority)
							.ToList();
					systems.ForEach(system => Systems.SystemsDictionary.Add(system.GetType(), system));
					CoroutineRunner.Start(Coroutine(systems, callback));
				};
			}

			private IEnumerator Coroutine(List<ISystem> systems, Action<bool> callback)
			{
				foreach (var system in systems)
					yield return system.Run();
				_keepWaiting = false;
				Ready = true;
				callback?.Invoke(Ready);
			}
        }
        
		public const string SceneName = "Systems";

		private static readonly Dictionary<Type, ISystem> SystemsDictionary = new();

		public static Scene ActiveSystemsScene { get; private set; }
		public static bool Ready { get; private set; }

		/// <summary>
		/// Initializes the Systems architecture. To wait for the operations to complete, yield on this call in a coroutine.
		/// </summary>
		public static CustomYieldInstruction Load(Action<bool> callback = null) => Load(SceneName, callback);

		/// <summary>
		/// Initializes the Systems architecture with the specified Systems scene name.
		/// To wait for the operations to complete, yield on this call in a coroutine.
		/// </summary>
		public static CustomYieldInstruction Load(string systemsSceneName, Action<bool> callback = null)
		{
			if (Ready)
			{
				callback?.Invoke(Ready);
				return null;
			}
			return new SystemOperation(systemsSceneName, callback);
		}

		/// <summary>
		/// Once all the Systems have been loaded, they can be accessed through calls to this function.
		/// </summary>
		/// <typeparam name="T">The type of the system to retrieve. Must derive from ISystem.</typeparam>
		public static T GetSystem<T>() where T : ISystem
		{
			if (!Ready) throw new InvalidOperationException("Systems are not Ready. Ensure Load() has been called before this.");
			return (T)SystemsDictionary[typeof(T)];
		}

		/// <summary>
		/// Once all the Systems have been loaded, they can be accessed through calls to this function.
		/// Returns True if the system was found, False otherwise.
		/// </summary>
		/// <typeparam name="T">The type of the system to retrieve. Must derive from ISystem.</typeparam>
		public static bool TryGetSystem<T>(out T system) where T : ISystem
		{
			system = default;
			if (!Ready)
			{
				Debug.LogError("Systems are not Ready. Ensure Load() has been called before this.");
				return false;
			}

			var result = SystemsDictionary.TryGetValue(typeof(T), out var s);
			system = (T)s;
			return result;
		}
    }
}