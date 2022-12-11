using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

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
			"SetDangerWrapper",
			new[] { EventInterface.EventInfoType, typeof(int) }
		);
		setDangerDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			setDangerInfo,
			dangerManagerInstance,
			EventInterface.EventInfoType,
			typeof(int)
		);

		var getDangerInfo = AccessTools.DeclaredMethod(
			dangerType,
			"GetDangerWrapper",
			new[] { EventInterface.EventInfoType }
		);
		getDangerDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			getDangerInfo,
			dangerManagerInstance,
			EventInterface.EventInfoType,
			typeof(int?)
		);
	}

	/// <summary>
	/// Sets the danger for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> to set the danger for</param>
	/// <param name="danger">The new <see cref="Danger"/> for the event</param>
	public void SetDanger([NotNull] EventInfo eventInfo, Danger danger)
	{
		setDangerDelegate(eventInfo.EventInfoInstance, danger);
	}

	/// <summary>
	/// Gets the danger for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> to get the danger of</param>
	/// <returns>The <see cref="Danger"/> of the event, if it exists, otherwise <c>null</c></returns>
	[CanBeNull]
	public Danger? GetDanger([NotNull] EventInfo eventInfo)
	{
		return (Danger?) getDangerDelegate(eventInfo.EventInfoInstance);
	}
}
