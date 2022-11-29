using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchLib;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class ConditionsManager
{
	private static ConditionsManager instance;

	public static ConditionsManager Instance => instance ??= new ConditionsManager();

	private ConditionsManager()
	{
	}

	// every event can have conditions that take the event's data and returns a bool
	// all conditions must return true for the event to be considered active
	private readonly Dictionary<EventInfo, ConditionRef> conditions = new();

	/// <summary>
	/// Adds a condition for the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> of the event to add a condition to</param>
	/// <param name="condition">The condition to be run to determine if the event is active</param>
	public void AddCondition([NotNull] EventInfo eventInfo, [NotNull] Func<object, bool> condition)
	{
		if (conditions.TryGetValue(eventInfo, out var condRef))
		{
			condRef.Condition += condition;
		}
		else
		{
			conditions.Add(eventInfo, new ConditionRef(condition));
		}
	}

	/// <summary>
	/// Runs the conditions for an event to determine if the event is active
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to check</param>
	/// <param name="data">The data to pass to each condition</param>
	/// <returns><c>true</c> if no conditions exist or if all conditions passed, <c>false</c> if any condition failed.</returns>
	public bool CheckCondition([NotNull] EventInfo eventInfo, [CanBeNull] object data)
	{
		if (conditions.TryGetValue(eventInfo, out var condRef))
		{
			foreach (var cond in condRef.Condition.GetInvocationList())
			{
				var result = (bool) cond.DynamicInvoke(data);
				if (!result)
				{
					return false;
				}
			}
		}

		// Either no condition or the conditions all passed
		return true;
	}
}
