using System.Collections.Generic;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventManager = EventLib.EventManager;

namespace ONITwitchCore;

public static class DefaultCommands
{
	public const string CommandNamespace = "ONITwitch."; 
	
	public static void SetupCommands()
	{
		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;
		var dangerInst = DangerManager.Instance;

		var eventA = eventInst.RegisterEvent(NamespaceId("eventA"), "Event A");
		eventInst.AddListenerForEvent(eventA, EventAListener);
		dataInst.AddDataForEvent(eventA, new List<string> {"uwu", "owo"});

		var eventB = eventInst.RegisterEvent(NamespaceId("eventB"), "Event B");
		eventInst.AddListenerForEvent(eventB, EventBListener);
		dataInst.AddDataForEvent(eventB, 42.0d);
		dangerInst.SetDanger(eventB, Danger.Deadly);
		
		var eventC = eventInst.RegisterEvent(NamespaceId("eventC"), "Event C");
		var eventD = eventInst.RegisterEvent(NamespaceId("eventD"), "Event D");
		
		// TODO: properly generate options

		TwitchDeckManager.Instance.AddToDeck(eventA);
		TwitchDeckManager.Instance.AddToDeck(eventB);
		TwitchDeckManager.Instance.AddToDeck(eventC);
		TwitchDeckManager.Instance.AddToDeck(eventD);

		var condInst = ConditionsManager.Instance;
		condInst.AddCondition(
			eventA,
			data =>
			{
				Debug.Log("cond: does A contain \"uwu\"");
				if (data is List<string> eventData)
				{
					return eventData.Contains("uwu");
				}

				return false;
			}
		);
		condInst.AddCondition(
			eventA,
			_ =>
			{
				Debug.Log("Running cond A2 (pass)");
				return true;
			}
		);
	}

	private static void EventAListener(object d)
	{
		if (d is List<string> data)
		{
			Debug.Log("event A data:");
			foreach (var s in data)
			{
				Debug.Log($"\t{s}");
			}
		}
		else
		{
			Debug.LogWarning($"event A expected a List<string>, found {d.GetType()}");
		}
	}

	private static void EventBListener(object d)
	{
		if (d is double data)
		{
			Debug.Log($"event B data: {data}");
		}
		else
		{
			Debug.LogWarning($"event B expected a double, found {d.GetType()}");
		}
	}

	private static string NamespaceId(string id)
	{
		return CommandNamespace + id;
	}
}