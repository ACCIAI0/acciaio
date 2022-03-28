using System.Collections;
using UnityEngine;

namespace Acciaio
{
	public static class CoroutineRunner
	{
		private class StaticCoroutineRunner : MonoBehaviour { }

		private static StaticCoroutineRunner _runner;

		public static Coroutine Start(IEnumerator routine)
		{
			if (_runner == null)
			{
				_runner = new GameObject($"[{typeof(CoroutineRunner)}]").AddComponent<StaticCoroutineRunner>();
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
	}
}