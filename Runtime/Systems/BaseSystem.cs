using System.Collections;
using UnityEngine;

namespace Acciaio
{
	public abstract class BaseSystem<TSystem> : Singleton<TSystem>, ISystem where TSystem : BaseSystem<TSystem>
	{
		public abstract bool IsRunning { get; }

		private IEnumerator InternalRun()
		{
			yield return StartCoroutine(RunRoutine());
			if (!IsRunning) 
				Debug.LogWarning($"{GetType().Name}.Run() was called but IsRunning is still false.");
		}

		private IEnumerator InternalShutdown()
		{
			yield return StartCoroutine(ShutdownRoutine());
			if (IsRunning)
				Debug.LogWarning($"{GetType().Name}.Shutdown() was called but IsRunning is still true.");
		}

		/// <summary>
		/// Subclasses should override this method instead of the Awake method.
		/// </summary>
		protected override void AwakeOverride() { }

		/// <summary>
		/// Subclasses should override this method instead of the OnDestroy method.
		/// </summary>
        protected override void OnDestroyOverride() { }

		/// <summary>
		/// Override to add your initialization logic in form of a Coroutine. 
		/// If the method ends with success, it make sure IsRunning will return true.
		/// </summary>
		protected abstract IEnumerator RunRoutine();

		/// <summary>
		/// Override to add your shutdown logic in form of a Coroutine. 
		/// It should always make sure IsRunning will return false after a call to Shutdown().
		/// </summary>
		protected abstract IEnumerator ShutdownRoutine();

		
		public YieldInstruction Run()
		{
			enabled = true;
			return StartCoroutine(InternalRun());
		}
		
		public YieldInstruction Shutdown()
		{
			enabled = true;
			return StartCoroutine(InternalShutdown());
		}
	}
}