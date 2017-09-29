using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a machine too create a few treasure box, and only one box is correct randomly, according to the
//  
public class SlotMachine {
	private float[] setRate = { 0.8f, 0.2f, 0.3f };  // the rate of each treasure box, can edit by user
	private int boxCount;

	private float rateAccount = 0f;

	public int BoxCount { get { return setRate.Length; } }
	public float[] SetRate { get { return setRate; } }

	public bool Init()
	{
		boxCount = setRate.Length;
		rateAccount = 0;
		for (int i = 0; i < boxCount; i++)
		{
			rateAccount += setRate[i];
		}
		
		for (int i = 0; i < boxCount; i++)
		{
			setRate[i] = setRate[i] / rateAccount;
		}

		return true;
	}

	public int CreateResult()
	{
		int ret = -1;
		float random = Random.Range(0f, 1f);
		float temp = 0;
		for (int i = 0; i < boxCount; i++)
		{
			temp += setRate[i];
			if (temp >= random)
			{
				ret = i;
				break;
			}
		}

		if (ret < 0)
			ret = boxCount - 1;

		return ret + 1;
	}
}
