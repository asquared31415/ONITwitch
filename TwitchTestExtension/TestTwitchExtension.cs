using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;
using ONITwitchLib.Core;
using ProcGen;

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

		var genConfig = new NoisePocketDimensionGeneration(
			3f,
			SubWorld.ZoneType.Metallic,
			new List<SimHashes>
			{
				SimHashes.Gold,
				SimHashes.Aluminum,
				SimHashes.Copper,
			},
			0.1f,
			0.1f
		);
		PocketDimensionGenerator.AddGenerationConfig(genConfig);
	}
}

[HarmonyPatch(typeof(PauseScreen), "OnShow")]
public static class ToastTest
{
	[UsedImplicitly]
	public static void Postfix()
	{
		if (Components.LiveMinionIdentities.Items.Count > 0)
		{
			var minion = Components.LiveMinionIdentities.Items.GetRandom();
			Debug.Log($"targeting {minion}");
			ToastManager.InstantiateToastWithGoTarget(
				"Test Toast",
				$"this is a test toast targeting {minion}",
				minion.gameObject
			);
		}
		else
		{
			Debug.LogWarning("Unable to find a minion to spawn a toast on");
		}
	}
}

internal record struct ExtData(bool Thing);
