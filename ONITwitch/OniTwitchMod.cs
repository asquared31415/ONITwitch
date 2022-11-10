using EventLib;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;

namespace ONITwitch;

[UsedImplicitly]
public class OniTwitchMod : UserMod2
{
	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);
		var eventInfo = EventManager.Instance.RegisterEvent("aaa");
		Debug.Log("Triggering event after init");
		EventManager.Instance.TriggerEvent(eventInfo);
		Debug.Log("adding one listener");
		EventManager.Instance.AddListenerForEvent(eventInfo, _ => { Debug.Log("Listener1");});
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
		Debug.Log("unregistering event 2");
		EventManager.Instance.RemoveListenerForEvent(eventInfo, Listener2);
		Debug.Log("triggering again");
		EventManager.Instance.TriggerEvent(eventInfo);
	}
}
