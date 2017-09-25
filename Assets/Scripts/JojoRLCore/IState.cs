using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JojoRLCore
{
	public class IStateMonitor
	{

	}

	public class IState
	{
		private IStateMonitor subStateMonitor;
		private IActionMonitor actionMonitor;
		
		virtual public int GetKey()
		{
			return 0;
		}


	}
}
