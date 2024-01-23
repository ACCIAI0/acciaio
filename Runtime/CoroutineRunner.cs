using System.Collections;
using UnityEngine;

namespace Acciaio
{
	public static class CoroutineRunner
	{
		private sealed class Runner : MonoBehaviour
		{
			private Runner() { }
		}

		private sealed class DelayedAction : CancelableDelay
		{
			private readonly System.Action _action;

			public DelayedAction(System.Action action, float delay) : base(delay) => _action = action;

			protected override IEnumerator Execution()
			{
				_action?.Invoke();
				yield break;
			}
		}

		public abstract class CancelableDelay : CustomYieldInstruction
		{
			private readonly float _secondsDelay;
			private Coroutine _routine;

			public bool IsDone => _routine == null;

			public override bool keepWaiting => !IsDone;

			protected CancelableDelay(float delay)
			{
				_secondsDelay = delay;
				_routine = Start(Delay());
			}

			private IEnumerator Delay()
			{
				yield return new WaitForSeconds(_secondsDelay);
				
				var inner = Execution();
				while(inner.MoveNext()) yield return inner.Current;

				_routine = null;
			}

			protected abstract IEnumerator Execution();

			public void Cancel() => Stop(_routine);
		}

		private static MonoBehaviour _runner;

		public static Coroutine Start(IEnumerator routine)
		{
			const string runnerGameObjectName = "[ACCIAIO] COROUTINE RUNNER";
			
			if (_runner == null)
			{
				_runner = new GameObject(runnerGameObjectName).AddComponent<Runner>();
				Object.DontDestroyOnLoad(_runner.gameObject);
			}
			_runner.gameObject.SetActive(true);
			_runner.enabled = true;
			return _runner.StartCoroutine(routine);
		}

		public static void Stop(Coroutine c)
		{
			if (_runner == null) return;
			_runner.StopCoroutine(c);
		}

		public static CancelableDelay ExecuteAfterSeconds(System.Action action, float delay) 
			=> new DelayedAction(action, delay);
	}
}