using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class DataManager
{
	private readonly object dataManagerInstance;
	private readonly Action<object, object> setDataForEventDelegate;
	private readonly Func<object, object> getDataForEventDelegate;

	internal DataManager(object inst)
	{
		dataManagerInstance = inst;
		var managerType = dataManagerInstance.GetType();
		var setDataInfo = AccessTools.DeclaredMethod(
			managerType,
			"SetDataForEvent",
			new[] { EventInterface.EventInfoType, typeof(object) }
		);
		setDataForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			setDataInfo,
			dataManagerInstance,
			EventInterface.EventInfoType,
			typeof(object)
		);
		var getDataInfo = AccessTools.DeclaredMethod(
			managerType,
			"GetDataForEvent",
			new[] { EventInterface.EventInfoType }
		);
		getDataForEventDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			getDataInfo,
			dataManagerInstance,
			EventInterface.EventInfoType,
			typeof(object)
		);
	}

	/// <summary>
	/// Sets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to modify</param>
	/// <param name="data">The new data for the event</param>
	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		setDataForEventDelegate(info.EventInfoInstance, data);
	}

	/// <summary>
	/// Gets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to get data for</param>
	/// <returns>The data for the event, if it exists, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public object GetDataForEvent([NotNull] EventInfo info)
	{
		return getDataForEventDelegate(info.EventInfoInstance);
	}
}
