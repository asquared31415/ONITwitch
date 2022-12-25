using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchCore.Config;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class TwitchDeckManager
{
	private static TwitchDeckManager instance;

	[NotNull] public static TwitchDeckManager Instance => instance ??= new TwitchDeckManager();

	private readonly VarietyShuffler<EventInfo> shuffler = new();

	private List<EventInfo> items = new();
	private int headIdx;

	private TwitchDeckManager()
	{
	}

	public void AddToDeck([NotNull] EventInfo eventInfo, int count, [CanBeNull] string groupName)
	{
		if (groupName == null)
		{
			AddToDeck(eventInfo, count);
		}
		else
		{
			var group = shuffler.GetGroup(groupName);
			if (group != null)
			{
				group.AddItem(eventInfo, count);
			}
			else
			{
				shuffler.AddGroup(groupName, new VarietyShuffler<EventInfo>.Group((eventInfo, count)));
			}

			items = shuffler.GetShuffled();
			headIdx = 0;
		}
	}

	public void AddToDeck([NotNull] EventInfo eventInfo, int count)
	{
		shuffler.AddItem(eventInfo, count);
		items = shuffler.GetShuffled();
		headIdx = 0;
	}

	public void AddToDeck([NotNull] EventInfo eventInfo)
	{
		AddToDeck(eventInfo, 1);
	}

	[Obsolete("This overload is kept only for binary compatibility reasons", true)]
	public void AddToDeck([NotNull] IEnumerable<EventInfo> eventInfos)
	{
		foreach (var eventInfo in eventInfos)
		{
			AddToDeck(eventInfo);
		}
	}

	public void RemoveEvent([NotNull] EventInfo eventInfo)
	{
		var noGroupName = shuffler.GetItemDefaultGroupName(eventInfo);
		RemoveGroup(noGroupName);
	}

	public void RemoveGroup([NotNull] string groupName)
	{
		shuffler.RemoveGroup(groupName);
		items = shuffler.GetShuffled();
		headIdx = 0;
	}

	[CanBeNull]
	public VarietyShuffler<EventInfo>.Group GetGroup([NotNull] string groupName)
	{
		return shuffler.GetGroup(groupName);
	}

	[CanBeNull]
	public EventInfo Draw()
	{
		const int maxDrawAttempts = 1000;

		var condInst = ConditionsManager.Instance;
		var dataInst = DataManager.Instance;
		var dangerInst = DangerManager.Instance;

		for (var attempts = 0; attempts < maxDrawAttempts; attempts++)
		{
			var entry = DrawEntry();
			// no danger assigned or danger within the expected range is okay
			var danger = dangerInst.GetDanger(entry);
			if ((danger == null) || ((MainConfig.Instance.ConfigData.MinDanger <= danger.Value) &&
									 (danger.Value <= MainConfig.Instance.ConfigData.MaxDanger)))
			{
				var data = dataInst.GetDataForEvent(entry);
				var condition = condInst.CheckCondition(entry, data);
				if (condition)
				{
					return entry;
				}
			}
		}

		Debug.LogWarning("[Twitch Integration] Unable to draw a command");
		return null;
	}

	private EventInfo DrawEntry()
	{
		if (items.Count == 0)
		{
			throw new Exception("Cannot draw from empty random deck");
		}

		var ret = items[headIdx];
		headIdx += 1;
		if (headIdx >= items.Count)
		{
			items = shuffler.GetShuffled();
			headIdx = 0;
		}

		return ret;
	}
}
