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
		public const string SCENE_NAME = "Systems";

		private class NoOp : CustomYieldInstruction
		{
			public static readonly NoOp Instance = new NoOp();

			public override bool keepWaiting => false;

			private NoOp() {}
		}

        private class SystemOperation : CustomYieldInstruction
        {
			private readonly AsyncOperation _op;
			private bool _keepWaiting = true;

            public override bool keepWaiting => _keepWaiting;

			public SystemOperation()
			{
				_op = SceneManager.LoadSceneAsync(SCENE_NAME, LoadSceneMode.Additive);
				_op.completed += op =>
				{
					var scene = SceneManager.GetSceneByName(SCENE_NAME);
					if (!scene.IsValid())
					{
						Debug.LogError($"Systems scene '{SCENE_NAME}' was not loaded correctly. Maybe it wasn't added to the build order?");
						return;
					}

					List<ISystem> systems = scene.GetRootGameObjects()
							.Select(o => o.GetComponent<ISystem>())
							.Where(o => o != null)
							.ToList();
					systems.ForEach(s => Systems._systems.Add(s.GetType(), s));
					CoroutineRunner.Start(Coroutine(systems));
				};
			}

			private IEnumerator Coroutine(IEnumerable<ISystem> systems)
			{
				foreach (var s in systems)
					yield return s.Run();
				_keepWaiting = false;
				Ready = true;
			}
        }

		private static readonly Dictionary<Type, ISystem> _systems = new Dictionary<Type, ISystem>();

		public static bool Ready { get; private set; }

		/// <summary>
		/// Initializes the Systems architecture. To wait for the operations to complete, yield on this call in a coroutine.
		/// </summary>
		public static CustomYieldInstruction Load()
		{
			if (Ready) return NoOp.Instance;
			return new SystemOperation();
		}

		/// <summary>
		/// Once all the Systems have been loaded, they can be accessed through calls to this function.
		/// </summary>
		/// <typeparam name="T">The type of the system to retrieve. Must derive from ISystem.</typeparam>
		public static T GetSystem<T>() where T : ISystem
		{
			if (!Ready) throw new InvalidOperationException("Systems are not Ready. Ensure Load() has been called before this.");
			return (T)_systems[typeof(T)];
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

			bool result = _systems.TryGetValue(typeof(T), out ISystem s);
			system = (T)s;
			return result;
		}
    }
}