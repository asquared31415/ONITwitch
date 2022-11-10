using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class EventManager
{
	// the EventManager instance from the event lib, but type erased as an object
	private readonly object eventManagerInstance;

	private readonly Func<string, object> registerEventDelegate;
	private readonly Func<string, object> getEventByIdDelegate;
	private readonly MethodInfo addListenerForEventInfo;
	private readonly MethodInfo removeListenerForEventInfo;
	private readonly MethodInfo triggerEventInfo;

	internal EventManager(object instance)
	{
		eventManagerInstance = instance;
		var eventType = eventManagerInstance.GetType();
		var registerInfo = AccessTools.DeclaredMethod(eventType, "RegisterEvent", new[] { typeof(string) });
		registerEventDelegate = DelegateUtil.CreateDelegate<Func<string, object>>(eventManagerInstance, registerInfo);
		var getByIdInfo = AccessTools.DeclaredMethod(eventType, "GetEventByID", new[] { typeof(string) });
		getEventByIdDelegate = DelegateUtil.CreateDelegate<Func<string, object>>(eventManagerInstance, getByIdInfo);
		addListenerForEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"AddListenerForEvent",
			new[] { EventInterface.EventInfoType, typeof(Action<object>) }
		);
		removeListenerForEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"RemoveListenerForEvent",
			new[] { EventInterface.EventInfoType, typeof(Action<object>) }
		);
		triggerEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"TriggerEvent",
			new[] { EventInterface.EventInfoType }
		);
	}

	[NotNull]
	[ContractAnnotation("id:null => halt")]
	public EventInfo RegisterEvent([NotNull] string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			throw new ArgumentException("ID must not be null or whitespace", nameof(id));
		}

		var output = registerEventDelegate(id);
		if (output.GetType() != EventInterface.EventInfoType)
		{
			throw new Exception("event register type");
		}

		return new EventInfo(output);
	}

	[CanBeNull]
	[ContractAnnotation("id:null => halt")]
	public EventInfo GetEventById([NotNull] string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			throw new ArgumentException("ID must not be null or whitespace", nameof(id));
		}

		var output = getEventByIdDelegate(id);
		if (output.GetType() != EventInterface.EventInfoType)
		{
			throw new Exception("event by id type");
		}

		return new EventInfo(output);
	}

	public void AddListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<object> listener)
	{
		addListenerForEventInfo.Invoke(eventManagerInstance, new[] { eventInfo.EventInfoInstance, listener });
	}

	public void RemoveListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<object> listener)
	{
		removeListenerForEventInfo.Invoke(eventManagerInstance, new[] { eventInfo.EventInfoInstance, listener });
	}

	public void TriggerEvent([NotNull] EventInfo eventInfo)
	{
		triggerEventInfo.Invoke(eventManagerInstance, new[] { eventInfo.EventInfoInstance });
	}
}
