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
}

[HarmonyPatch(typeof(Db))]
[HarmonyPatch("Initialize")]
[HarmonyAfter("asquared31415.TwitchIntegration")]
public static class Db_Initialize_Patch
{
	public static void Postfix()
	{
		if (!TwitchModInfo.TwitchIsPresent)
		{
			Debug.LogWarning("Twitch not enabled");
			return;
		}

		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;

		var (extEvent, extEventGroup) = EventGroup.DefaultSingleEventGroup("ExtEvent", 4, "Extended Event");
		extEvent.AddListener(
			data =>
			{
				Debug.Log("Triggered ext event");
				Debug.Log(data);
			}
		);
		// add condition
		extEvent.AddCondition(data => data is ExtData { Thing: true });
		extEvent.Danger = Danger.None;

		// set up the data
		dataInst.SetDataForEvent(extEvent, new ExtData(true));

		// register the group into the deck
		deckInst.AddGroup(extEventGroup);

		var rainPrefabType = Type.GetType("ONITwitchCore.Commands.RainPrefabCommand, ONITwitch");
		var commandBase = new CommandBase(rainPrefabType);

		var (extRain, extRainGroup) = EventGroup.DefaultSingleEventGroup("CustomRainEvent", 1, "Custom Rain Event");
		extRain.AddListener(commandBase.GetRunAction());

		dataInst.SetDataForEvent(
			extRain,
			new Dictionary<string, object> { { "PrefabId", "PropFacilityCouch" }, { "Count", 10.0d } }
		);

		deckInst.AddGroup(extRainGroup);

		// create a custom pocket dimension
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

		var group = TwitchDeckManager.Instance.GetGroup("aaaaaa");
		if (group != null)
		{
			group.AddEvent("aaaa", 0);
		}
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
