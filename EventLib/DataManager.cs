using System.Collections.Generic;
using JetBrains.Annotations;

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

	public void SetDataForEvent([NotNull] EventInfo info, object data)
	{
		storedData[info] = data;
	}

	[CanBeNull]
	public object GetDataForEvent([NotNull] EventInfo info)
	{
		return storedData.TryGetValue(info, out var data) ? data : null;
	}
}
