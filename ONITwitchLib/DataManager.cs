using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Core;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

/// <summary>
/// Provides methods to manipulate data of <see cref="EventInfo"/>s
/// </summary>
[PublicAPI]
public class DataManager
{
	/// <summary>
	/// The instance of the data manager.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public static DataManager Instance => instance ??= new DataManager();

	/// <summary>
	/// Sets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to modify.</param>
	/// <param name="data">The new data for the event.</param>
	[PublicAPI]
	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		setDataForEventDelegate(info.EventInfoInstance, data);
	}

	/// <summary>
	/// Gets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to get data for.</param>
	/// <returns>The data for the event, if it exists, otherwise <see langword="null"/></returns>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
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
			var prop = AccessTools.Property(CoreTypes.DataManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != CoreTypes.DataManagerType)
			{
				throw new Exception(
					$"The Instance property on {CoreTypes.DataManagerType.AssemblyQualifiedName} does not return an instance of {CoreTypes.DataManagerType}"
				);
			}

			// no argument because it's static property
			dataManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var inst = dataManagerInstanceDelegate();

		setDataForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(CoreTypes.DataManagerType, "SetDataForEvent"),
			inst,
			CoreTypes.EventInfoType,
			typeof(object)
		);
		getDataForEventDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(CoreTypes.DataManagerType, "GetDataForEvent"),
			inst,
			CoreTypes.EventInfoType,
			typeof(object)
		);
	}
}
