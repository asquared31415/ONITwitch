using System.Collections.Generic;
using EventLib;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchLib;
using EventManager = EventLib.EventManager;

namespace ONITwitch;

[UsedImplicitly]
public class OniTwitchMod : UserMod2
{
	public override void OnLoad(Harmony harmony)
	{
		Debug.Log(typeof(OniTwitchMod).AssemblyQualifiedName);
		base.OnLoad(harmony);
		var eventInfo = EventManager.Instance.RegisterEvent("aaa");
		Debug.Log("Triggering event after init");
		var dictData = new Dictionary<string, object>();
		EventManager.Instance.TriggerEvent(eventInfo, dictData);
		Debug.Log("adding one listener");
		EventManager.Instance.AddListenerForEvent(eventInfo, _ => { Debug.Log("Listener1"); });
		Debug.Log("triggering again");
		EventManager.Instance.TriggerEvent(eventInfo, dictData);
		Debug.Log("adding second listener");

		void Listener2(object _)
		{
			Debug.Log("Listener2");
		}

		EventManager.Instance.AddListenerForEvent(eventInfo, Listener2);
		Debug.Log("triggering again");
		EventManager.Instance.TriggerEvent(eventInfo, dictData);
		Debug.Log("unregistering event 2 via reflection");
		var eventManager = EventInterface.GetEventManagerInstance();
		var info = eventManager.GetEventById("aaa")!;
		eventManager.RemoveListenerForEvent(info, Listener2);

		Debug.Log("triggering again normally");
		EventManager.Instance.TriggerEvent(eventInfo, dictData);
		Debug.Log("triggering again via reflection");
		eventManager.TriggerEvent(info, dictData);
		Debug.Log("adding printer listener via reflection");

		void Printer(object data)
		{
			if (data is List<string> d)
			{
				foreach (var s in d)
				{
					Debug.Log(s);
				}
			}
		}
		eventManager.AddListenerForEvent(info, Printer);
		
		Debug.Log("adding list data for event");
		DataManager.Instance.AddDataForEvent(eventInfo, new List<string>(){"abc", "def"});
		Debug.Log("triggering via reflection with data");
		var d = (List<string>) DataManager.Instance.GetDataForEvent(eventInfo);
		eventManager.TriggerEvent(info, d);
		Debug.Log("triggering again normally");
		EventManager.Instance.TriggerEvent(eventInfo, d);
	}
}
