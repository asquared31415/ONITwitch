using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitchLib;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class TwitchDeckManager
{
	private static TwitchDeckManager instance;

	[NotNull]
	public static TwitchDeckManager Instance => instance ??= new TwitchDeckManager();

	private readonly RandomDeck<EventInfo> deck = new(new List<EventInfo>());
	
	private TwitchDeckManager() {}
	
	public void AddToDeck([NotNull] EventInfo eventInfo) {
		deck.AddAndShuffle(eventInfo);
	}

	public void AddToDeck([NotNull] IEnumerable<EventInfo> eventInfos)
	{
		deck.AddAndShuffle(eventInfos);
	}

	public EventInfo Draw()
	{
		return deck.DrawEntry();
	}
}
