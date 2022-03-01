using UnityEngine;

namespace Acciaio
{
	[DisallowMultipleComponent]
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static bool systemIsShuttingDown = false;
		
		public static T Instance { get; private set; } = null;

		public static bool Exists => Instance != null;

		[SerializeField]
		private bool _preventReplacement = false;
		[SerializeField]
		private bool _dontDestroyOnLoad = false;

		private bool _isBeingReplaced = false;

		private void Awake() 
		{
			if (Instance != null && Instance != this) 
			{
				if (Instance._preventReplacement)
				{
					Debug.LogWarning("Cannot replace an Instance of Singleton " + GetType().Name);
					Destroy(gameObject);
					return;
				}

				Debug.LogWarning("Replacing an Instance of Singleton " + GetType().Name);
				Instance._isBeingReplaced = true;
				Destroy(Instance.gameObject);
			}

			if (systemIsShuttingDown)
			{
				Destroy(gameObject);
				return;
			}

			if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

			Instance = (T)this;

			AwakeOverride();
		}

		private void OnDestroy()
		{
			if (Instance == this) Instance = null;
			systemIsShuttingDown = !_isBeingReplaced;

			OnDestroyOverride();
		}

		/// <summary>
		/// This method must be defined instead of Awake() in order to
		/// reduce the possibility to remove the singleton logic.
		/// </summary>
		protected abstract void AwakeOverride();

		/// <summary>
		/// This method must be defined instead of OnDestroy() in order to
		/// reduce the possibility to remove the singleton logic.
		/// </summary>
		protected abstract void OnDestroyOverride();
	}
}