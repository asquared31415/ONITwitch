using System.Collections.Generic;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using ONITwitchCore.Config;

// ReSharper disable once CheckNamespace : this must be kept for compatibility reasons (TODO: fix for 1.0?)
namespace EventLib;

/// <summary>
/// Provides methods to manipulate data of <see cref="EventInfo"/>s
/// </summary>
[PublicAPI]
public class DataManager
{
	private static DataManager instance;
	private readonly Dictionary<EventInfo, object> storedData = new();

	/// <summary>
	/// The instance of the data manager.
	/// </summary>
	[PublicAPI]
	[NotNull]
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
	/// <param name="info">The <see cref="EventInfo"/> for the event to modify.</param>
	/// <param name="data">The new data for the event.</param>
	[PublicAPI]
	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		// overwrite with the user provided data if applicable
		var config = UserCommandConfigManager.Instance.GetConfig(info.EventNamespace, info.Id);
		storedData[info] = config != null ? config.Data : data;
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
		return storedData.TryGetValue(info, out var data) ? data : null;
	}
}
