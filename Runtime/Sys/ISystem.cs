using UnityEngine;

namespace Acciaio.Sys
{
	public interface ISystem
	{
		/// <summary>
		/// Specifies the priority of activation of this System. Lower values make a System of higher priority.
		/// </summary>
		int Priority { get; }
		
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

        YieldInstruction AllSystemsReady();
	}
}