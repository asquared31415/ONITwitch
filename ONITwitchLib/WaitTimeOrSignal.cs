using System;
using UnityEngine;

namespace OniTwitchLib;

public class WaitTimeOrSignal : CustomYieldInstruction
{
	private readonly float waitTime;
	private readonly Func<bool> waitTrigger;
	private float accumulatedTime;

	public WaitTimeOrSignal(float time, Func<bool> trigger)
	{
		waitTime = time;
		accumulatedTime = 0;
		waitTrigger = trigger;
	}

	public override bool keepWaiting
	{
		get
		{
			accumulatedTime += Time.unscaledDeltaTime;
			var stopEarly = waitTrigger();
			return (accumulatedTime < waitTime) && !stopEarly;
		}
	}
}
