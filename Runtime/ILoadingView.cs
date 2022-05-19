using UnityEngine;

namespace Acciaio
{
	public interface ILoadingView
	{
		public YieldInstruction Show();
		public YieldInstruction Hide();
	}
}