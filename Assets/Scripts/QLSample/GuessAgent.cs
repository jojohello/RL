using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JojoRLCore;

public class GuessAgent : IAgent
{
	override public void Init()
	{
		root = (IStateMonitor)(new GuessMonitor());
	}

	public override IActionMonitor CreateActionMonitor(IAgentData data)
	{
		GuessActionMonitor newActionMonitor = new GuessActionMonitor();

		GuessData guessData = data as GuessData;
		int boxCount = guessData.boxCount;
		for(int i=1; i<= boxCount; i++)
		{
			GuessAction newAction = new GuessAction(i);
			newActionMonitor.actionDict.Add(i, newAction);
		}

		newActionMonitor.AfterAddActions();

		return newActionMonitor;
	}
}


public class GuessData: IAgentData
{
	public int boxCount = 0;
	public int lastResult = 1;
	public int guessResult = 0;
}