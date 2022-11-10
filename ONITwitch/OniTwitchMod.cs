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
		EventManager.Instance.TriggerEvent(eventInfo);
		Debug.Log("adding one listener");
		EventManager.Instance.AddListenerForEvent(eventInfo, _ => { Debug.Log("Listener1"); });
		Debug.Log("triggering again");
		EventManager.Instance.TriggerEvent(eventInfo);
		Debug.Log("adding second listener");

		void Listener2(object _)
		{
			Debug.Log("Listener2");
		}

		EventManager.Instance.AddListenerForEvent(eventInfo, Listener2);
		Debug.Log("triggering again");
		EventManager.Instance.TriggerEvent(eventInfo);
		Debug.Log("unregistering event 2 via reflection");
		var eventManager = EventInterface.GetEventManagerInstance();
		var info = eventManager.GetEventById("aaa");
		eventManager.RemoveListenerForEvent(info, Listener2);

		Debug.Log("triggering again normally");
		EventManager.Instance.TriggerEvent(eventInfo);
		Debug.Log("triggering again via reflection");
		eventManager.TriggerEvent(info);
		Debug.Log("adding third listener via reflection");
		eventManager.AddListenerForEvent(info, _ => Debug.Log("Listener3"));
		Debug.Log("triggering via reflection");
		eventManager.TriggerEvent(info);
		Debug.Log("triggering again normally");
		EventManager.Instance.TriggerEvent(eventInfo);
	}
}
