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
		deckInst.AddToDeck(extEvent, 3);

		// set the danger to none
		dangerInst.SetDanger(extEvent, Danger.None);

		var rainPrefabType = Type.GetType("ONITwitchCore.Commands.RainPrefabCommand, ONITwitch");
		var commandBase = new CommandBase(rainPrefabType);

		var customEvent = eventInst.RegisterEvent("CustomSpawnEvent", "Custom Spawn Event");

		var action = commandBase.GetRunAction();
		eventInst.AddListenerForEvent(customEvent, action);

		dataInst.SetDataForEvent(
			customEvent,
			new Dictionary<string, object> { { "PrefabId", "PropFacilityCouch" }, { "Count", 10.0d } }
		);

		deckInst.AddToDeck(customEvent, 1);
	}
}

internal record struct ExtData(bool Thing);
