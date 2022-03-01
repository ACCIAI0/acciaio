using UnityEngine;

namespace Acciaio
{
	public interface ILoadingView
	{
		public Coroutine Show();
		public Coroutine Hide();
	}
}