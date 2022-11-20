using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchLib;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class DangerManager
{
	private static DangerManager instance;
	public static DangerManager Instance => instance ??= new DangerManager();

	private DangerManager()
	{
	}

	private readonly Dictionary<EventInfo, Danger> dangerMap = new();

	public void SetDanger([NotNull] EventInfo eventInfo, Danger danger)
	{
		dangerMap[eventInfo] = danger;
	}

	[CanBeNull]
	public Danger? GetDanger([NotNull] EventInfo eventInfo)
	{
		if (dangerMap.TryGetValue(eventInfo, out var danger))
		{
			return danger;
		}

		return null;
	}
}
