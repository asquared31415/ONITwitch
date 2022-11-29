using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class ConditionsManager
{
	// the ConditionsManager instance from the core lib, but type erased as an object
	private readonly object conditionsManagerInstance;

	// fn(EventInfo, fn(object) -> bool)
	private readonly Action<object, Func<object, bool>> addConditionDelegate;

	// fn(EventInfo, object) -> bool
	private readonly Func<object, object, object> checkConditionDelegate;

	internal ConditionsManager(object inst)
	{
		conditionsManagerInstance = inst;
		var conditionsType = conditionsManagerInstance.GetType();

		var addConditionInfo = AccessTools.DeclaredMethod(
			conditionsType,
			"AddCondition",
			new[] { EventInterface.EventInfoType, typeof(Func<object, bool>) }
		);
		addConditionDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			addConditionInfo,
			conditionsManagerInstance,
			EventInterface.EventInfoType,
			typeof(Func<object, bool>)
		);

		var checkConditionInfo = AccessTools.DeclaredMethod(
			conditionsType,
			"CheckCondition",
			new[] { EventInterface.EventInfoType, typeof(object) }
		);
		checkConditionDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			checkConditionInfo,
			conditionsManagerInstance,
			EventInterface.EventInfoType,
			typeof(object),
			typeof(bool)
		);
	}

	/// <summary>
	/// Adds a condition for the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> of the event to add a condition to</param>
	/// <param name="condition">The condition to be run to determine if the event is active</param>
	public void AddCondition([NotNull] EventInfo eventInfo, [NotNull] Func<object, bool> condition)
	{
		addConditionDelegate(eventInfo.EventInfoInstance, condition);
	}

	/// <summary>
	/// Runs the conditions for an event to determine if the event is active
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to check</param>
	/// <param name="data">The data to pass to each condition</param>
	/// <returns><c>true</c> if no conditions exist or if all conditions passed, <c>false</c> if any condition failed.</returns>
	public bool CheckCondition([NotNull] EventInfo eventInfo, object data)
	{
		return (bool) checkConditionDelegate(eventInfo.EventInfoInstance, data);
	}
}
