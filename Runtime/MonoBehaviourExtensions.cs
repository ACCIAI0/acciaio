using UnityEngine;

namespace Acciaio
{
	public static class MonoBehaviourExtensions
	{
		public static T AssignAndGet<T>(this MonoBehaviour behaviour, ref T field, bool createIfMissing = false) where T : Component
		{
			if (field == null) field = behaviour.GetComponent<T>();
			if (field == null && createIfMissing) field = behaviour.gameObject.AddComponent<T>();
			return field;
		}
	}
}
