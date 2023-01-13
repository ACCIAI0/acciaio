using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acciaio
{
	public interface ILoadingView
	{
		public Scene Scene { get; }
		public YieldInstruction Show();
		public YieldInstruction Hide();
	}
}