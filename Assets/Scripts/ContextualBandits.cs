using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextualBandits : MonoBehaviour
{

	static private float[] ms_setRate = { 0.8f, 0.2f, 0.3f};
	static private int ms_count = ms_setRate.Length;
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

	private Ret[] m_stateAndRet;	// 通过状态表，得到上下文老虎机的效果
	private float[] m_rateCounter;	// 全部状态的概率统计起来，作为一个总得表示数，用来显示
	private int m_lastRet = -1;		// 上一次的结果
	private int m_calTime = 0;		// 计算总次数
	private int m_correctTime = 0;	// 结果正确的次数
	private float m_correctRate = 0;	// 准确率
	// Use this for initialization
	void Start()
	{
		float account = 0;
		for (int i = 0; i < ms_count; i++)
		{
			account += ms_setRate[i];
		}

		m_stateAndRet = new Ret[ms_count];
		m_rateCounter = new float[ms_count];
		for (int i = 0; i < ms_count; i++)
		{
			ms_setRate[i] = ms_setRate[i] / account;

			m_stateAndRet[i] = new Ret(ms_count);

			for(int j=0; j<ms_count; j++)
				m_stateAndRet[i].ret[j] = 1f / ms_count;
		}

		InitUI();

		m_lastTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	private void Update()
	{
		if (Time.realtimeSinceStartup - m_lastTime < m_delta)
			return;

		int ret = GetRandomResult(ms_setRate);
		if (m_lastRet < 0)
		{
			m_lastRet = ret;
			return;
		}

		int guessRet = GetRandomResult(m_stateAndRet[m_lastRet].ret);

		int r = 1;
		if (guessRet != ret)
			r = 0;

		m_stateAndRet[m_lastRet].ret[guessRet] = m_stateAndRet[m_lastRet].ret[guessRet] + ms_learnRate * (r - m_stateAndRet[m_lastRet].ret[guessRet]);
		ReCaculateRate(ref m_stateAndRet[m_lastRet].ret);
		CaculateRateCounter(ref m_rateCounter);
		m_lastRet = ret;

		// 
		if (guessRet == ret)
			m_correctTime++;
		m_calTime++;
		if(m_calTime >= 200)
		{
			m_correctRate = (float)m_correctTime / m_calTime;
			m_correctTime = 0;
			m_calTime = 0;
		}

		Refresh();
	}

	int GetRandomResult(float[] input)
	{
		int ret = 0;
		int count = input.Length;
		float account = 0;
		for (int i = 0; i < count; i++)
		{
			account += input[i];
		}

		float random = Random.Range(0f, account);
		float temp = 0;
		for (int i = 0; i < count; i++)
		{
			temp += input[i];
			if (temp >= random)
			{
				ret = i;
				break;
			}
		}

		return ret;
	}

	void ReCaculateRate(ref float[] rates)
	{
		float account = 0;
		for (int i = 0; i < ms_count; i++)
		{
			account += ms_setRate[i];
		}

		for (int i = 0; i < ms_count; i++)
		{
			rates[i] = rates[i] / account;
		}
	}

	void CaculateRateCounter(ref float[] rateCount)
	{
		float account = 0;
		for (int i = 0; i < ms_count; i++)
			rateCount[i] = 0;

		for(int i = 0; i<ms_count; i++)
		{
			for(int j=0; j<ms_count; j++)
			{
				account += m_stateAndRet[i].ret[j];
				rateCount[j] += m_stateAndRet[i].ret[j];
			}
		}

		for (int i = 0; i < ms_count; i++)
			rateCount[i] = rateCount[i] / account;
	}
	#region UI
	Text[] valueText;
	Text correctRateText;
	private void InitUI()
	{
		correctRateText = GameObject.Find("CollectRate").GetComponent<Text>();

		valueText = new Text[ms_count];
		GameObject parent = GameObject.Find("Box");
		for (int i = 0; i < ms_count; i++)
		{
			GameObject header = parent;
			if (i != 0)
			{
				header = GameObject.Instantiate(parent) as GameObject;
				header.transform.parent = parent.transform.parent;
			}

			Transform temp = header.transform.FindChild("SetValue");
			Text setValueText = temp.GetComponent<Text>();
			setValueText.text = ms_setRate[i].ToString();

			temp = header.transform.FindChild("RLValue");
			Text calValueText = temp.GetComponent<Text>();
			calValueText.text = m_rateCounter[i].ToString();
			valueText[i] = calValueText;
		}
	}

	private void Refresh()
	{
		correctRateText.text = m_correctRate.ToString();

		for (int i = 0; i < ms_count; i++)
		{
			valueText[i].text = m_rateCounter[i].ToString();
		}
	}
	#endregion

}

