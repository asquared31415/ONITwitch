using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EventLib;

public class EventManager
{
	private class RefActionWrapper
	{
		public Action<Dictionary<string, object>> Action;

		public RefActionWrapper(Action<Dictionary<string, object>> action)
		{
			Action = action;
		}
	}

	private static EventManager instance;

	// entries in this dictionary are never null
	private readonly Dictionary<string, RefActionWrapper> registeredEvents = new();

	public static EventManager Instance
	{
		get
		{
			instance ??= new EventManager();
			return instance;
		}
	}

	[NotNull]
	public EventInfo RegisterEvent([NotNull] string namespacedId)
	{
		if (!registeredEvents.ContainsKey(namespacedId))
		{
			registeredEvents.Add(namespacedId, new RefActionWrapper(delegate { }));
		}

		var eventInfo = new EventInfo(namespacedId);
		return eventInfo;
	}

	[CanBeNull]
	public EventInfo GetEventByID([NotNull] string id)
	{
		return registeredEvents.ContainsKey(id) ? new EventInfo(id) : null;
	}

	public void AddListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<Dictionary<string, object>> listener)
	{
		if (registeredEvents.TryGetValue(eventInfo.Id, out var val))
		{
			val.Action += listener;
		}
		else
		{
			throw new ArgumentException(
				$"The event {eventInfo.Id} has not yet been registered with RegisterEvent",
				nameof(eventInfo)
			);
		}
	}

	public void RemoveListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<Dictionary<string, object>> listener)
	{
		if (registeredEvents.TryGetValue(eventInfo.Id, out var val))
		{
			if (!val.Action.GetInvocationList().Contains(listener))
			{
				throw new ArgumentException(
					$"unable to remove listener from event {eventInfo.Id}",
					nameof(listener)
				);
			}

			val.Action -= listener;
		}
		else
		{
			throw new ArgumentException(
				$"The event {eventInfo.Id} has not yet been registered with RegisterEvent",
				nameof(eventInfo)
			);
		}
	}

	public void TriggerEvent([NotNull] EventInfo eventInfo, [NotNull] Dictionary<string, object> data)
	{
		if (registeredEvents.TryGetValue(eventInfo.Id, out var events))
		{
			events.Action.Invoke(data);
		}
		else
		{
			throw new ArgumentException(
				$"The event {eventInfo.Id} has not yet been registered with RegisterEvent",
				nameof(eventInfo)
			);
		}
	}
}
