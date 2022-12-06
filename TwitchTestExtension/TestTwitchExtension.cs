using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;
using ONITwitchLib.Core;

namespace TwitchTestExtension;

[UsedImplicitly]
public class TestTwitchExtension : UserMod2
{
	public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
	{
		base.OnAllModsLoaded(harmony, mods);
		if (!TwitchModInfo.TwitchIsPresent)
		{
			Debug.LogWarning("Twitch not enabled");
			return;
		}

		var eventInst = EventInterface.GetEventManagerInstance();
		var dataInst = EventInterface.GetDataManagerInstance();
		var conditionsInst = EventInterface.GetConditionsManager();
		var deckInst = EventInterface.GetDeckManager();
		var dangerInst = EventInterface.GetDangerManager();

		var extEvent = eventInst.RegisterEvent("ExtEvent", "Extended Event");
		eventInst.AddListenerForEvent(
			extEvent,
			data =>
			{
				Debug.Log("Triggered ext event");
				Debug.Log(data);
			}
		);

		dataInst.SetDataForEvent(extEvent, new ExtData(true));

		// add condition
		conditionsInst.AddCondition(
			extEvent,
			data => data is ExtData { Thing: true }
		);

		// add 1 copy and then 3 more copies to the deck
		deckInst.AddToDeck(extEvent);
		deckInst.AddToDeck(DeckUtils.RepeatList(extEvent, 3));

		// set the danger to none
		dangerInst.SetDanger(extEvent, Danger.None);

		var x = Type.GetType("ONITwitchCore.Commands.SpawnDupeCommand, ONITwitch");
		var commandInst = Activator.CreateInstance(x);
		var commandBase = new CommandBase(commandInst);
		var action = commandBase.GetRunAction();

		var customEvent = eventInst.RegisterEvent("CustomSpawnEvent", "Custom Spawn Event");
		eventInst.AddListenerForEvent(customEvent, action);
		deckInst.AddToDeck(DeckUtils.RepeatList(customEvent, 10));
	}
}

internal record struct ExtData(bool Thing);
