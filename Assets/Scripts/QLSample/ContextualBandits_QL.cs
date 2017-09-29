using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JojoRLCore;

public class ContextualBandits_QL : MonoBehaviour
{

	static private float ms_learnRate = 0.01f;

	private float m_delta = 0.01f;
	private float m_lastTime = 0f;

	class Ret
	{
		public float[] ret;

		public Ret(int count)
		{
			ret = new float[count];
		}
	};
	
	private float[] m_rateCounter;	// 全部状态的概率统计起来，作为一个总得表示数，用来显示
	private int m_calTime = 0;		// 计算总次数
	private int m_correctTime = 0;	// 结果正确的次数
	private float m_correctRate = 0;    // 准确率
	private int m_trainCount = 0;
	
	private SlotMachine slotMachine;
	private GuessData data;
	private GuessAgent agent;

	// Use this for initialization
	void Start()
	{
		slotMachine = new SlotMachine();
		slotMachine.Init();

		data = new GuessData();
		data.boxCount = slotMachine.BoxCount;

		agent = new GuessAgent();
		agent.Init();

		m_rateCounter = new float[slotMachine.BoxCount];

		InitUI();

		m_lastTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	private void Update()
	{
		if (Time.realtimeSinceStartup - m_lastTime < m_delta)
			return;

		int ret = slotMachine.CreateResult();
		
		agent.Excute(data);
		int guessRet = data.guessResult;

		if (guessRet == ret)
		{
			agent.AddValue(1);
		}else
		{
			agent.AddValue(0);
		}

		data.lastResult = ret;
		CaculateRateCounter(ref m_rateCounter);

		// 
		if (guessRet == ret)
			m_correctTime++;
		m_calTime++;
		m_trainCount++;
		if (m_calTime >= 200)
		{
			m_correctRate = (float)m_correctTime / m_calTime;
			m_correctTime = 0;
			m_calTime = 0;
		}

		Refresh();
	}

	void CaculateRateCounter(ref float[] rateCount)
	{
		int boxCount = slotMachine.BoxCount;
		float account = 0;
		for (int i = 0; i < boxCount; i++)
			rateCount[i] = 0;

		int endStateCount = agent.EndNodeStates.Count;
		for (int i = 1; i <= endStateCount; i++)
		{
			for (int j = 1; j <= boxCount; j++)
			{
				account += agent.EndNodeStates[i].ActionMonitor.actionDict[j].SelfRate;
				rateCount[j-1] += agent.EndNodeStates[i].ActionMonitor.actionDict[j].SelfRate;
			}
		}

		for (int i = 0; i < boxCount; i++)
			rateCount[i] = rateCount[i] / account;
	}

	#region UI
	Text[] valueText;
	Text correctRateText;
	Text trainCountText;
	private void InitUI()
	{
		correctRateText = GameObject.Find("CollectRate").GetComponent<Text>();
		trainCountText = GameObject.Find("TrainCount").GetComponent<Text>();

		int machineCount = slotMachine.BoxCount;
		valueText = new Text[machineCount];
		GameObject parent = GameObject.Find("Box");
		for (int i = 0; i < machineCount; i++)
		{
			GameObject header = parent;
			if (i != 0)
			{
				header = GameObject.Instantiate(parent) as GameObject;
				header.transform.parent = parent.transform.parent;
			}

			Transform temp = header.transform.Find("SetValue");
			Text setValueText = temp.GetComponent<Text>();
			setValueText.text = slotMachine.SetRate[i].ToString();

			temp = header.transform.Find("RLValue");
			Text calValueText = temp.GetComponent<Text>();
			calValueText.text = m_rateCounter[i].ToString();
			valueText[i] = calValueText;
		}
	}

	private void Refresh()
	{
		trainCountText.text = m_trainCount.ToString();
		correctRateText.text = m_correctRate.ToString();

		int boxCount = slotMachine.BoxCount;
		for (int i = 0; i < boxCount; i++)
		{
			valueText[i].text = m_rateCounter[i].ToString();
		}
	}
	#endregion

}

