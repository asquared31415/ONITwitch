using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class DataManager
{
	private readonly object dataManagerInstance;
	private readonly Action<object, object> addDataForEventDelegate;
	private readonly Func<object, object> getDataForEventDelegate;

	internal DataManager(object inst)
	{
		dataManagerInstance = inst;
		var managerType = dataManagerInstance.GetType();
		var addDataInfo = AccessTools.DeclaredMethod(
			managerType,
			"AddDataForEvent",
			new[] { EventInterface.EventInfoType, typeof(object) }
		);
		addDataForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			addDataInfo,
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

	public void AddDataForEvent([NotNull] EventInfo info, object data)
	{
		addDataForEventDelegate(info.EventInfoInstance, data);
	}

	[CanBeNull]
	public object GetDataForEvent([NotNull] EventInfo info)
	{
		return getDataForEventDelegate(info.EventInfoInstance);
	}
}
