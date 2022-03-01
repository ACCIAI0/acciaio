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

		public static bool Ready { get; private set; }

		public static CustomYieldInstruction Load()
		{
			if (Ready) return NoOp.Instance;
			return new SystemOperation();
		}
    }
}