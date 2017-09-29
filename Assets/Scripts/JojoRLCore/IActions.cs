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

		public Dictionary<int, IAction> actionDict = new Dictionary<int, IAction>();
		static private List<CaculateData> calData = new List<CaculateData>();
		private int curExcuteAction = -1;

		public void AfterAddActions()
		{
			float account = 0f;
			foreach (var key in actionDict.Keys)
			{
				if (actionDict[key].SelfValue > float.Epsilon)
					account += actionDict[key].SelfValue;
			}

			foreach (var key in actionDict.Keys)
			{
				if (actionDict[key].SelfValue > float.Epsilon)
					actionDict[key].SelfRate = actionDict[key].SelfValue / account;
				else
					actionDict[key].SelfRate = 0f;
			}
		}

		public void CumulationValue(int key, float value, float studyRate)
		{
			IAction action = null;
			if(actionDict.TryGetValue(key, out action))
			{
				action.SelfRate += studyRate * (value - action.SelfRate);
				UpdateActionRate();
			}
		}

		public int ConfirmAndExcuteAction(IAgentData agentData)
		{
			int retKey = -1;
			float maxValue = 0f;

			calData.Clear();
			foreach(var key in actionDict.Keys)
			{
				if (actionDict[key].SelfRate < float.Epsilon)
					continue;

				maxValue += actionDict[key].SelfRate;
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

		private void UpdateActionRate()
		{
			float account = 0f;
			foreach(var key in actionDict.Keys)
			{
				if (actionDict[key].SelfRate > float.Epsilon)
					account += actionDict[key].SelfRate;
			}

			foreach (var key in actionDict.Keys)
			{
				if (actionDict[key].SelfRate > float.Epsilon)
					actionDict[key].SelfRate = actionDict[key].SelfRate / account;
				else
					actionDict[key].SelfRate = 0f;
			}
		}
	}


	public class IAction
	{
		protected int id;
		protected float selfValue = 100f;  // base value, can be reduce, and will never be chosen when less then 0
		protected float selfRate = 0f;

		public float SelfRate
		{
			set { selfRate = value; }
			get { return selfRate; }
		}

		public float SelfValue
		{
			set { selfValue = value;}
			get{ return selfValue;}	
		}

		public int ID
		{
			set { id = value; }
			get { return id; }
		}

		virtual public int GetKey()
		{
			return id;
		}

		// what this action do! The params is not designed now, maybe i will package a class to do this
		virtual public void Excute(IAgentData data)
		{
			
		}
	}
}

