using System;

namespace ONITwitchLib;

/// <summary>
/// A wrapper for a Func&lt;object, bool&gt; for event conditions that can be used as a reference 
/// </summary>
public class ConditionRef
{
	/// <summary>
	/// The wrapped condition
	/// </summary>
	public Func<object, bool> Condition;

	public ConditionRef(Func<object, bool> condition)
	{
		Condition = condition;
	}
}
