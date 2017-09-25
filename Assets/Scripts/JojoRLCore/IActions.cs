using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JojoRLCore
{
	// One system is enough to have one actionMonitor. In this monitor, it contain all action that
	// can do in the game. And this monitor provide the way to chose the action. And each end noke
	// of the state tree show contain an ActionMonitor instance. 
	public class IActionMonitor
	{
		private class CaculateData
		{
			public int key;
			public float range;

			public CaculateData(int inputKey, float inputRange)
			{
				key = inputKey;
				range = inputRange;
			}
		}

		private Dictionary<int, IAction> actionDict = new Dictionary<int, IAction>();
		static private List<CaculateData> calData = new List<CaculateData>();

		public void CumulationValue(int key, float value)
		{
			IAction action = null;
			if(actionDict.TryGetValue(key, out action))
			{
				action.SelfValue += value;
			}
		}

		public int ConfirmAndExcuteAction(IAgentData agentData)
		{
			int retKey = -1;
			float maxValue = 0f;

			calData.Clear();
			foreach(var key in actionDict.Keys)
			{
				if (actionDict[key].SelfValue < 0)
					continue;

				maxValue += actionDict[key].SelfValue;
				CaculateData newData = new CaculateData(key, maxValue);
				calData.Add(newData);
			}

			int count = calData.Count;
			if (count <= 0)
				return -1;

			float curRange = Random.Range(0f, maxValue);
			int curIndex = 0;
			for(curIndex = 0; curIndex < count; curIndex++)
			{
				if(calData[curIndex].range < curRange)
					continue;

				retKey = calData[curIndex].key;
				actionDict[retKey].Excute(agentData);
				break;
			}

			if(curIndex == count)
			{
				--curIndex;
				retKey = calData[curIndex].key;
				actionDict[retKey].Excute(agentData);
			}

			return retKey;
		}
	}


	public class IAction
	{
		int id;
		float selfValue = 100;  // base value, can be reduce, and will never be chosen when less then 0

		public float SelfValue
		{
			set { selfValue = value;}
			get{ return selfValue;}	
		}

		virtual public int GetKey()
		{
			return 0;
		}

		// what this action do! The params is not designed now, maybe i will package a class to do this
		virtual public void Excute(IAgentData data)
		{
			
		}
	}
}

