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

	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		setDataForEventDelegate(info.EventInfoInstance, data);
	}

	[CanBeNull]
	public object GetDataForEvent([NotNull] EventInfo info)
	{
		return getDataForEventDelegate(info.EventInfoInstance);
	}
}
