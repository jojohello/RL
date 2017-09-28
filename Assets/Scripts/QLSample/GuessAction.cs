using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JojoRLCore;

public class GuessActionMonitor :IActionMonitor
{

}

public class GuessAction : IAction
{ 
	public GuessAction(int boxIndex)
	{
		ID = boxIndex;
	}

	override public void Excute(IAgentData data)
	{
		GuessData guessData = data as GuessData;
		guessData.guessResult = ID;
	}
}
