using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JojoRLCore;

public class GuessMonitor : IStateMonitor {

	// Use this for initialization
	public override int GetKeyByData(IAgentData data)
	{
		GuessData guessData = data as GuessData;
		return guessData.lastResult;
	}

	public override IState CreateNewState(IAgentData data)
	{
		int machineResult = GetKeyByData(data);
		IState newState = new GuessState(machineResult);

		return newState;
	}
}

public class GuessState: IState
{
	private int id;

	public GuessState(int machineResult)
	{
		id = machineResult;
	}

	override public int GetKey()
	{
		return id;
	}
}