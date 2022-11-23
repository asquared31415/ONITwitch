using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EventLib;

public class EventManager
{
	private class RefActionWrapper
	{
		public Action<object> Action;

		public RefActionWrapper(Action<object> action)
		{
			Action = action;
		}
	}

	private static EventManager instance;

	// entries in this dictionary are never null
	private readonly Dictionary<string, RefActionWrapper> registeredEvents = new();
	private readonly Dictionary<string, string> idNameMap = new();

	public static EventManager Instance
	{
		get
		{
			instance ??= new EventManager();
			return instance;
		}
	}

	[NotNull]
	public EventInfo RegisterEvent([NotNull] string namespacedId, [CanBeNull] string friendlyName = null)
	{
		if (registeredEvents.ContainsKey(namespacedId))
		{
			throw new Exception($"id {namespacedId} already registered");
		}

		registeredEvents.Add(namespacedId, new RefActionWrapper(delegate { }));
		idNameMap.Add(namespacedId, friendlyName);
		var eventInfo = new EventInfo(namespacedId);
		return eventInfo;
	}

	public void RenameEvent([NotNull] EventInfo eventInfo, [NotNull] string friendlyName)
	{
		if (idNameMap.ContainsKey(eventInfo.Id))
		{
			idNameMap[eventInfo.Id] = friendlyName;
		}
	}

	[CanBeNull]
	public EventInfo GetEventByID([NotNull] string id)
	{
		return registeredEvents.ContainsKey(id) ? new EventInfo(id) : null;
	}

	public void AddListenerForEvent(
		[NotNull] EventInfo eventInfo,
		[NotNull] Action<object> listener
	)
	{
		registeredEvents[eventInfo.Id].Action += listener;
	}

	public void RemoveListenerForEvent(
		[NotNull] EventInfo eventInfo,
		[NotNull] Action<object> listener
	)
	{
		var val = registeredEvents[eventInfo.Id];
		if (!val.Action.GetInvocationList().Contains(listener))
		{
			throw new ArgumentException(
				$"unable to remove listener from event {eventInfo.Id}",
				nameof(listener)
			);
		}

		val.Action -= listener;
	}

	public void TriggerEvent([NotNull] EventInfo eventInfo, object data)
	{
		registeredEvents[eventInfo.Id].Action.Invoke(data);
	}
}
