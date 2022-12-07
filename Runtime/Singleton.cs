#pragma warning disable RCS1158

using UnityEngine;

namespace Acciaio
{
	[DisallowMultipleComponent]
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		public static T Instance { get; private set; }
		public static bool Exists => Instance != null;

		[Header("Singleton")]
		[SerializeField]
		private bool _preventReplacement;
		[SerializeField]
		private bool _dontDestroyOnLoad;

		protected void Awake()
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
				Destroy(Instance.gameObject);
			}

			if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

			Instance = (T)this;

			AwakeOverride();
		}

		protected void OnDestroy()
		{
			if (Instance == this) Instance = null;
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
#pragma warning restore RCS1158