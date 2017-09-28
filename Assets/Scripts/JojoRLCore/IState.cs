using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JojoRLCore
{
	public class IStateMonitor
	{
		Dictionary<int, IState> states = new Dictionary<int, IState>();
		int curAction = -1;

		public int CurAction
		{
			set { curAction = value; }
			get { return curAction; }
		}

		public virtual int GetKeyByData(IAgentData data)
		{
			return 0;
		}

		public virtual IState CreateNewState(IAgentData data)
		{
			return null;
		}

		public int Excute(IAgentData data, IAgent agent)
		{
			int key = GetKeyByData(data);
			if(states.ContainsKey(key) == false)
			{
				IState newState = CreateNewState(data);
				newState.Init(data);
				states.Add(key, newState);

				if(newState.IsEndNode())
				{
					newState.WorldIndex = agent.AddEndNodeState(newState);
					newState.ActionMonitor = agent.CreateActionMonitor(data);
				}
			}

			if (states[key].IsEndNode())
			{
				states[key].ActionMonitor.ConfirmAndExcuteAction(data);
				return states[key].WorldIndex;
			}	
			else
				return states[key].SubStateMonitor.Excute(data, agent);
		}
	}

	public class IState
	{
		private int worldIndex = -1;
		private IStateMonitor subStateMonitor;
		private IActionMonitor actionMonitor;
		
		public int WorldIndex
		{
			set { worldIndex = value; }
			get { return worldIndex; }
		}

		public IActionMonitor ActionMonitor
		{
			set { actionMonitor = value; }
			get { return actionMonitor; }
		}

		public IStateMonitor SubStateMonitor
		{
			set { subStateMonitor = value; }
			get { return subStateMonitor; }
		}

		virtual public int GetKey()
		{
			return 0;
		}
		
		virtual public bool Init(IAgentData fromData)
		{
			return true;
		}

		virtual public void createSubStates()
		{

		}

		virtual public void CreateActions(IAgentData data)
		{

		}

		public bool IsEndNode()
		{
			return subStateMonitor == null;
		}
	}
}
