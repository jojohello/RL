using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JojoRLCore
{
	public class IAgent
	{
		const float StudyRate = 0.01f;

		private int startIndex = 1;
		protected Dictionary<int, IState> states = new Dictionary<int, IState>(); // add the end Node to this dictionary, may it easy to reference
		protected IStateMonitor root;

		public int curState = -1;
		public int curAction = -1;
		public int lastState = -1;
		public int lastAction = -1;

		bool isInit = false;

		public Dictionary<int, IState> EndNodeStates { get { return states; } }

		// build the tree structures here
		virtual public void Init()
		{
		
		}

		public virtual IActionMonitor CreateActionMonitor(IAgentData data)
		{
			return null;
		}

		public int AddEndNodeState(IState newState)
		{
			if (newState == null)
				return -1;

			states[startIndex] = newState;
			startIndex++;

			return startIndex - 1;
		}

		public void Excute(IAgentData data)
		{
			if (root == null)
				return;

			lastState = curState;
			lastAction = curAction;
			root.Excute(data, this);
		}

		public void AddValue(float v)
		{
			IState state = null;
			if (states.TryGetValue(curState, out state) == false)
				return;

			state.ActionMonitor.CumulationValue(curAction, v, StudyRate);
		}
	}
}

