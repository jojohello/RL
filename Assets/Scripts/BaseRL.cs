using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseRL: MonoBehaviour {

	static private float[] ms_setRate = { 0.1f, 0.2f, 0.3f, 0.5f, 0.6f };
	static private int ms_count = ms_setRate.Length;
	static private float ms_learnRate = 0.0001f;

	private float m_delta = 0.5f;
	private float m_curTime = 0f;
	private float[] m_ret;
	// Use this for initialization
	void Start () {
		float account = 0;
		for(int i=0; i<ms_count; i++)
		{
			account += ms_setRate[i];
		}

		m_ret = new float[ms_count];
		for(int i=0; i<ms_count; i++)
		{
			ms_setRate[i] = ms_setRate[i] / account;
			m_ret[i] = 1f / ms_count;
		}

		InitUI();

		m_curTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	private void Update()
	{
		if (Time.realtimeSinceStartup - m_curTime < m_delta)
			return;

		int ret = GetRandomResult(ms_setRate);
		int guessRet = GetRandomResult(m_ret);

		int r = 1;
		if (guessRet != ret)
			r = 0;

		m_ret[guessRet] = m_ret[guessRet] + ms_learnRate * (r - m_ret[guessRet]);
		ReCaculateRate(ref m_ret);

		Refresh();
	}

	int GetRandomResult(float[] input)
	{
		int ret = 0;
		int count = input.Length;
		float account = 0;
		for(int i=0; i<count; i++)
		{
			account += input[i];
		}

		float random = Random.Range(0f, account);
		float temp = 0;
		for(int i=0; i<count; i++)
		{
			temp += input[i];
			if(temp >= random)
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

		for(int i = 0; i < ms_count; i++)
		{
			rates[i] = rates[i] / account;
		}
	}
#region UI
	Text[] valueText;
	private void InitUI()
	{
		valueText = new Text[ms_count];
		GameObject parent = GameObject.Find("Box");
		for(int i = 0; i<ms_count; i++)
		{
			GameObject header = parent;
			if(i != 0)
			{
				header = GameObject.Instantiate(parent) as GameObject;
				header.transform.parent = parent.transform.parent;
			}

			Transform temp = header.transform.FindChild("SetValue");
			Text setValueText = temp.GetComponent<Text>();
			setValueText.text = ms_setRate[i].ToString();

			temp = header.transform.FindChild("RLValue");
			Text calValueText = temp.GetComponent<Text>();
			calValueText.text = m_ret[i].ToString();
			valueText[i] = calValueText;
		}
	}

	private void Refresh()
	{
		for(int i=0; i<ms_count; i++)
		{
			valueText[i].text = m_ret[i].ToString();
		}
	}
#endregion
	
}
