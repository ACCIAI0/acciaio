using UnityEngine;

namespace Acciaio
{
	public interface ISystem
	{
		YieldInstruction Run();
		
		YieldInstruction Shutdown();
	}
}