using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib;

/// <summary>
/// A class for use in Unity coroutines that continues execution after a duration has elapsed, or a function signals
/// to continue, whichever comes first.
/// </summary>
[PublicAPI]
public class WaitTimeOrSignal : CustomYieldInstruction
{
	/// <summary>
	/// Initializes the time and signal. 
	/// </summary>
	/// <param name="time">The amount of time, in unscaled real-time seconds to wait.</param>
	/// <param name="signal">
	/// A function that returns <see langword="true"/> if the wait should end early,
	/// or <see langword="false"/> otherwise.
	/// </param>
	[PublicAPI]
	public WaitTimeOrSignal(float time, Func<bool> signal)
	{
		accumulatedTime = 0;
		waitTime = time;
		waitSignal = signal;
	}

	/// <summary>
	/// Whether this delay should continue to wait.
	/// </summary>
	public override bool keepWaiting
	{
		get
		{
			accumulatedTime += Time.unscaledDeltaTime;
			var stopEarly = waitSignal();
			return (accumulatedTime < waitTime) && !stopEarly;
		}
	}

	private readonly float waitTime;
	private readonly Func<bool> waitSignal;
	private float accumulatedTime;
}
