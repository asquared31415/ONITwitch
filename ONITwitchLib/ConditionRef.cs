using System;

namespace ONITwitchLib;

public class ConditionRef
{
	public Func<object, bool> Condition;

	public ConditionRef(Func<object, bool> condition)
	{
		Condition = condition;
	}
}
