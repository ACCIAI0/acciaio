using UnityEngine;

namespace Acciaio
{
	public interface ISystem
	{
		/// <summary>
		/// True after the Run() method is called. False if it's never called or after Shutdown() is called.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Starts up the System. It's yieldable in coroutines.
		/// </summary>
		YieldInstruction Run();
		
		/// <summary>
		/// Shuts down the System. It's yieldable in coroutines.
		/// </summary>
		YieldInstruction Shutdown();
	}
}