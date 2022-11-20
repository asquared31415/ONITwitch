using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class DangerManager
{
	private readonly object dangerManagerInstance;

	// fn(EventInfo, Danger)
	private readonly Action<object, object> setDangerDelegate;

	// fn(EventInfo) -> Danger?
	private readonly Func<object, object> getDangerDelegate;

	internal DangerManager(object inst)
	{
		dangerManagerInstance = inst;
		var dangerType = dangerManagerInstance.GetType();

		var setDangerInfo = AccessTools.DeclaredMethod(
			dangerType,
			"SetDanger",
			new[] { EventInterface.EventInfoType, typeof(Danger) }
		);
		setDangerDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			setDangerInfo,
			dangerManagerInstance,
			EventInterface.EventInfoType,
			typeof(Danger)
		);

		var getDangerInfo = AccessTools.DeclaredMethod(dangerType, "GetDanger", new[] { EventInterface.EventInfoType });
		getDangerDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			getDangerInfo,
			dangerManagerInstance,
			EventInterface.EventInfoType,
			typeof(Danger?)
		);
	}

	public void SetDanger([NotNull] EventInfo eventInfo, Danger danger)
	{
		setDangerDelegate(eventInfo.EventInfoInstance, danger);
	}

	public Danger? GetDanger([NotNull] EventInfo eventInfo)
	{
		return (Danger?) getDangerDelegate(eventInfo.EventInfoInstance);
	}
}
