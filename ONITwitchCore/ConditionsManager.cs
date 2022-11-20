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

	public void AddCondition([NotNull] EventInfo info, [NotNull] Func<object, bool> condition)
	{
		if (conditions.TryGetValue(info, out var condRef))
		{
			condRef.Condition += condition;
		}
		else
		{
			conditions.Add(info, new ConditionRef(condition));
		}
	}

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
