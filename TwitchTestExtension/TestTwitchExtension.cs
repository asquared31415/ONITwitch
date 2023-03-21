using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;
using ONITwitchLib.Core;
using ONITwitchLib.Logger;

namespace TwitchTestExtension;

[UsedImplicitly]
public class TestTwitchExtension : UserMod2
{
}

[HarmonyPatch(typeof(Db))]
[HarmonyPatch("Initialize")]
[HarmonyAfter("asquared31415.TwitchIntegration")]
// ReSharper disable once InconsistentNaming
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

		void GroupChanged(EventGroup group)
		{
			Log.Info($"Group changed, total weight: {group.TotalWeight}");
		}

		Log.Info("Adding group changed listener");
		extGroup.OnGroupChanged += GroupChanged;
		extGroup.SetWeight(manualGroupEvent, 10);
		extGroup.SetWeight(manualGroupEvent, 1);
		extGroup.OnGroupChanged -= GroupChanged;
		Log.Info("Removing group changed listener");
		extGroup.SetWeight(manualGroupEvent, 10);
		extGroup.SetWeight(manualGroupEvent, 1);
		Log.Info("Done changing weights");

		var group = TwitchDeckManager.Instance.GetGroup("TestExtGroup");
		group?.AddEvent("TestExtEvent", 0);
	}
}

internal record struct ExtData(bool Thing);
