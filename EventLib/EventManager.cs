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

	/// <summary>
	/// Registers an event with the event system.
	/// </summary>
	/// <param name="namespacedId">The ID of the event, which should be namespaced to avoid collisions</param>
	/// <param name="friendlyName">The user facing name to display</param>
	/// <returns>A <see cref="EventInfo"/> representing the event that has been registered.</returns>
	/// <exception cref="Exception">The ID <paramref name="namespacedId"/> is already registered.</exception>
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

	/// <summary>
	/// Changes the user-facing name for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to be changed</param>
	/// <param name="friendlyName">The new name for the event</param>
	public void RenameEvent([NotNull] EventInfo eventInfo, [NotNull] string friendlyName)
	{
		if (idNameMap.ContainsKey(eventInfo.Id))
		{
			idNameMap[eventInfo.Id] = friendlyName;
		}
	}

	/// <summary>
	/// Gets the user-facing name for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event</param>
	/// <returns>The friendly name, if it exists, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public string GetFriendlyName([NotNull] EventInfo eventInfo)
	{
		return idNameMap.TryGetValue(eventInfo.Id, out var name) ? name : null;
	}

	/// <summary>
	/// Gets an <see cref="EventInfo"/> for the specified ID, if the ID is registered.
	/// </summary>
	/// <param name="id">The ID to look for</param>
	/// <returns>An <see cref="EventInfo"/> representing the event, if the ID is found, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public EventInfo GetEventByID([NotNull] string id)
	{
		return registeredEvents.ContainsKey(id) ? new EventInfo(id) : null;
	}

	/// <summary>
	/// Adds a listener to the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to listen to</param>
	/// <param name="listener">The listener to call when the event is triggered</param>
	public void AddListenerForEvent(
		[NotNull] EventInfo eventInfo,
		[NotNull] Action<object> listener
	)
	{
		registeredEvents[eventInfo.Id].Action += listener;
	}

	/// <summary>
	/// Removes a listener from the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to remove from</param>
	/// <param name="listener">The listener to be removed from the event</param>
	/// <exception cref="ArgumentException"><paramref name="listener"/> was not listening to the event</exception>
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

	/// <summary>
	/// Triggers an event with the passed data.  This calls all listeners of the event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to trigger</param>
	/// <param name="data">The data to be passed to all listeners of the event</param>
	public void TriggerEvent([NotNull] EventInfo eventInfo, object data)
	{
		registeredEvents[eventInfo.Id].Action.Invoke(data);
	}

	[NotNull]
	public List<EventInfo> GetAllRegisteredEvents()
	{
		return registeredEvents.Keys.Select(s => new EventInfo(s)).ToList();
	}
}
