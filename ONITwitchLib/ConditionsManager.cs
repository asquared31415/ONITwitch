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

	public void AddCondition([NotNull] EventInfo eventInfo, [NotNull] Func<object, bool> condition)
	{
		addConditionDelegate(eventInfo.EventInfoInstance, condition);
	}

	public bool CheckCondition([NotNull] EventInfo eventInfo, object data)
	{
		return (bool) checkConditionDelegate(eventInfo.EventInfoInstance, data);
	}
}
