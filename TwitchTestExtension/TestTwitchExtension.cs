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
public class TestTwitchExtension : UserMod2;

[HarmonyPatch(typeof(Db), "Initialize")]
[HarmonyAfter(TwitchModInfo.StaticID)]
// ReSharper disable once InconsistentNaming
public static class Db_Initialize_Patch
{
	[UsedImplicitly]
	public static void Postfix()
	{
		if (!TwitchModInfo.TwitchIsPresent)
		{
			Log.Warn("[Test Extension Events] Twitch not enabled");
			return;
		}

		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;

		// Add a completely custom event
		var (extEvent, extEventGroup) = EventGroup.DefaultSingleEventGroup("LogEvent", 4, "Extra Event");
		extEvent.AddListener(
			data => { Log.Info($"Triggered ext event with data {data}"); }
		);
		// add condition so the event only runs when its data is true
		extEvent.AddCondition(([NotNull] data) => (bool) data);
		dataInst.SetDataForEvent(extEvent, true);
		// This event causes no danger
		extEvent.Danger = Danger.None;
		// register the group into the deck
		deckInst.AddGroup(extEventGroup);


		// Add an event that crashes to demonstrate the event crash handler
		var (crash, crashGroup) = EventGroup.DefaultSingleEventGroup("Crash", 0, "[DO NOT USE] CRASH EVENT");
		crash.AddListener(_ => throw new Exception("Explicit crash from a test event"));
		deckInst.AddGroup(crashGroup);


		// manual group creation
		var extGroup = EventGroup.GetOrCreateGroup("TwitchExtGroup");
		var manualGroupEvent = extGroup.AddEvent("ManuallyAddedEvent", 1, "Manual Group Event");
		manualGroupEvent.Danger = Danger.None;
		manualGroupEvent.AddCondition(_ => false);
		manualGroupEvent.AddListener(
			_ => { ToastManager.InstantiateToast("Toast", "This toast was manually triggered by the dev tools!"); }
		);
		deckInst.AddGroup(extGroup);


		Log.Info("Adding group changed listener");
		extGroup.OnGroupChanged += GroupChanged;
		extGroup.SetWeight(manualGroupEvent, 10);
		extGroup.SetWeight(manualGroupEvent, 1);
		extGroup.OnGroupChanged -= GroupChanged;
		Log.Info("Removing group changed listener");
		extGroup.SetWeight(manualGroupEvent, 10);
		extGroup.SetWeight(manualGroupEvent, 1);
		Log.Info("Done changing weights");


		// Reflecting for a type that exists and using it in a custom event
		// note: this is less reliable than rewriting code because events may change
		var rainPrefabType = Type.GetType("ONITwitch.Commands.RainPrefabCommand, ONITwitch")!;
		var commandBase = new CommandBase(rainPrefabType);
		var (extRain, extRainGroup) = EventGroup.DefaultSingleEventGroup("CustomRainEvent", 1, "Custom Rain Event");
		// Use the original event's run action as the action for this event too. You could also compose one or more events
		// run action together or add code with this
		extRain.AddListener(commandBase.GetRunAction());
		// This data must be careful to follow the expected format to the command to not crash
		dataInst.SetDataForEvent(
			extRain,
			new Dictionary<string, object> { { "PrefabId", "PropFacilityCouch" }, { "Count", 10.0d } }
		);
		deckInst.AddGroup(extRainGroup);


		// Attempting to get a group that may or may not exist, and modify it
		// In this case the group does not exist, so nothing will happen
		var group = TwitchDeckManager.Instance.GetGroup("TestExtGroupDOES_NOT_EXIST");
		group?.AddEvent("TestExtEvent", 0);
		return;

		// Group manipulation and on change event
		static void GroupChanged([NotNull] EventGroup group)
		{
			Log.Info($"Group changed, total weight: {group.TotalWeight}");
		}
	}
}
