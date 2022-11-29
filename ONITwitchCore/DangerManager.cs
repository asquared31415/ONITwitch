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

	/// <summary>
	/// Sets the danger for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> to set the danger for</param>
	/// <param name="danger">The new <see cref="Danger"/> for the event</param>
	public void SetDanger([NotNull] EventInfo eventInfo, Danger danger)
	{
		dangerMap[eventInfo] = danger;
	}

	/// <summary>
	/// Gets the danger for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> to get the danger of</param>
	/// <returns>The <see cref="Danger"/> of the event, if it exists, otherwise <c>null</c></returns>
	[CanBeNull]
	public Danger? GetDanger([NotNull] EventInfo eventInfo)
	{
		if (dangerMap.TryGetValue(eventInfo, out var danger))
		{
			return danger;
		}

		return null;
	}

	[UsedImplicitly]
	private void SetDangerWrapper([NotNull] EventInfo eventInfo, int danger)
	{
		SetDanger(eventInfo, (Danger) danger);
	}

	[CanBeNull]
	[UsedImplicitly]
	private int? GetDangerWrapper([NotNull] EventInfo eventInfo)
	{
		return (int?) GetDanger(eventInfo);
	}
}
