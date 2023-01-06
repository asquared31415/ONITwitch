using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

public class DataManager
{
	/// <summary>
	/// Gets the instance of the data manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static DataManager Instance => instance ??= new DataManager();

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

	private static DataManager instance;
	private static Func<object> dataManagerInstanceDelegate;

	private readonly Action<object, object> setDataForEventDelegate;
	private readonly Func<object, object> getDataForEventDelegate;

	private DataManager()
	{
		if (dataManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(EventInterface.DataManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != EventInterface.DataManagerType)
			{
				throw new Exception(
					$"The Instance property on {EventInterface.DataManagerType.AssemblyQualifiedName} does not return an instance of {EventInterface.DataManagerType}"
				);
			}

			// no argument because it's static property
			dataManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var inst = dataManagerInstanceDelegate();

		setDataForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(EventInterface.DataManagerType, "SetDataForEvent"),
			inst,
			EventInterface.EventInfoType,
			typeof(object)
		);
		getDataForEventDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(EventInterface.DataManagerType, "GetDataForEvent"),
			inst,
			EventInterface.EventInfoType,
			typeof(object)
		);
	}
}
