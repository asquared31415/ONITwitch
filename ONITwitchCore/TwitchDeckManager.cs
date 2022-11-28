using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchCore.Config;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class TwitchDeckManager
{
	private static TwitchDeckManager instance;

	[NotNull] public static TwitchDeckManager Instance => instance ??= new TwitchDeckManager();

	private readonly RandomDeck<EventInfo> deck = new(new List<EventInfo>());

	private TwitchDeckManager()
	{
	}

	public void AddToDeck([NotNull] EventInfo eventInfo)
	{
		deck.AddAndShuffle(eventInfo);
	}

	public void AddToDeck([NotNull] IEnumerable<EventInfo> eventInfos)
	{
		deck.AddAndShuffle(eventInfos);
	}

	[CanBeNull]
	public EventInfo Draw()
	{
		const int maxDrawAttempts = 100;

		var condInst = ConditionsManager.Instance;
		var dataInst = DataManager.Instance;
		var dangerInst = DangerManager.Instance;

		for (var attempts = 0; attempts < maxDrawAttempts; attempts++)
		{
			var entry = deck.DrawEntry();
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

	public void RemoveAll([NotNull] EventInfo eventInfo)
	{
		deck.RemoveAllAndShuffle(eventInfo);
	}
}
