using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;
using ONITwitchLib.Core;
using ONITwitchLib.Logger;
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
	[UsedImplicitly]
	public static void Postfix()
	{
		if (!TwitchModInfo.TwitchIsPresent)
		{
			Log.Warn("[Test Ext Events] Twitch not enabled");
			return;
		}

		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;

		var (crash, crashGroup) = EventGroup.DefaultSingleEventGroup("ExtCrash", 0, "[DO NOT USE] CRASH EVENT");
		crash.AddListener(
			_ =>
			{
				[NotNull]
				string FakeNotNull()
				{
					return null!;
				}

				// throws NRE
				var unused = FakeNotNull().Length;
			}
		);
		deckInst.AddGroup(crashGroup);

		var (extEvent, extEventGroup) = EventGroup.DefaultSingleEventGroup("ExtEvent", 4, "Extended Event");
		extEvent.AddListener(
			data => { Log.Info($"Triggered ext event with data {data}"); }
		);
		// add condition
		extEvent.AddCondition(data => data is ExtData { Thing: true });
		extEvent.Danger = Danger.None;

		// set up the data
		dataInst.SetDataForEvent(extEvent, new ExtData(true));

		// register the group into the deck
		deckInst.AddGroup(extEventGroup);

		var rainPrefabType = Type.GetType("ONITwitchCore.Commands.RainPrefabCommand, ONITwitch")!;
		var commandBase = new CommandBase(rainPrefabType);

		var (extRain, extRainGroup) = EventGroup.DefaultSingleEventGroup("CustomRainEvent", 1, "Custom Rain Event");
		extRain.AddListener(commandBase.GetRunAction());

		dataInst.SetDataForEvent(
			extRain,
			new Dictionary<string, object> { { "PrefabId", "PropFacilityCouch" }, { "Count", 10.0d } }
		);

		deckInst.AddGroup(extRainGroup);

		// manual group creation
		var extGroup = EventGroup.GetOrCreateGroup("ExtGroup");
		var manualGroupEvent = extGroup.AddEvent("ManualEvent", 1, "Manual Group Event");
		manualGroupEvent.Danger = Danger.None;
		manualGroupEvent.AddCondition(_ => false);
		manualGroupEvent.AddListener(
			_ => { ToastManager.InstantiateToast("Toast", "This toast was manually triggered by the dev tools!"); }
		);
		deckInst.AddGroup(extGroup);

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

internal record struct ExtData(bool Thing);
