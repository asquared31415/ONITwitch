using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;

namespace TwitchTestExtension;

public class TestTwitchExtension : UserMod2
{
}

[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
public static class Db_Init_Patch
{
	[UsedImplicitly]
	public static void Postfix()
	{
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

		dataInst.AddDataForEvent(extEvent, new ExtData(true));

		/*
		// Modify event A to have an extra entry in its data
		var eventA = eventInst.GetEventById("ONITwitch.eventA")!;
		var eventAData = (List<string>) dataInst.GetDataForEvent(eventA);
		eventAData.Add("Ext data!");
		*/
		
		// add and check conditions
		conditionsInst.AddCondition(
			extEvent,
			data => data is ExtData
		);
		var eventEnabled = conditionsInst.CheckCondition(extEvent, dataInst.GetDataForEvent(extEvent));
		Debug.Log($"is ext event condition pass: {eventEnabled}");

		// add 1 copy and then 3 more copies to the deck
		deckInst.AddToDeck(extEvent);
		deckInst.AddToDeck(DeckUtils.RepeatList(extEvent, 3));

		// set the danger to none
		dangerInst.SetDanger(extEvent, Danger.None);

		/*
		// make sure that default event B danger is deadly
		var eventB = eventInst.GetEventById("ONITwitch.eventB")!;
		var eventBDanger = dangerInst.GetDanger(eventB);
		Debug.Log(eventBDanger == Danger.Deadly);
		*/
	}
}

internal record struct ExtData(bool Thing);
