using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchCore.Config;

namespace EventLib;

public class DataManager
{
	private static DataManager instance;
	private readonly Dictionary<EventInfo, object> storedData = new();

	public static DataManager Instance
	{
		get
		{
			instance ??= new DataManager();
			return instance;
		}
	}

	/// <summary>
	/// Sets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to modify</param>
	/// <param name="data">The new data for the event</param>
	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		// overwrite with the user provided data if applicable
		var config = UserCommandConfigManager.Instance.GetConfig(info.EventNamespace, info.Id);
		storedData[info] = config != null ? config.Data : data;
	}

	/// <summary>
	/// Gets the data for an event.
	/// </summary>
	/// <param name="info">The <see cref="EventInfo"/> for the event to get data for</param>
	/// <returns>The data for the event, if it exists, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public object GetDataForEvent([NotNull] EventInfo info)
	{
		return storedData.TryGetValue(info, out var data) ? data : null;
	}
}
