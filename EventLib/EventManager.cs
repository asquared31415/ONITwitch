using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

	// a helper to look up a mod's static id from its assembly
	// populated once on first use
	private Dictionary<Assembly, string> assemblyStaticIdMap = null;

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
	/// <param name="id">The ID of the event.  The ID will be automatically namespaced with the static ID of the mod.</param>
	/// <param name="friendlyName">The user facing name to display</param>
	/// <returns>A <see cref="EventInfo"/> representing the event that has been registered.</returns>
	/// <exception cref="Exception">The ID <paramref name="id"/> is already registered.</exception>
	[NotNull]
	public EventInfo RegisterEvent([NotNull] string id, [CanBeNull] string friendlyName = null)
	{
		if (registeredEvents.ContainsKey(id))
		{
			throw new Exception($"id {id} already registered");
		}

		if (assemblyStaticIdMap == null)
		{
			assemblyStaticIdMap = new Dictionary<Assembly, string>();
			foreach (var mod in Global.Instance.modManager.mods)
			{
				var modStaticID = mod.staticID;
				var loadedData = mod.loaded_mod_data;
				if (loadedData != null)
				{
					foreach (var assembly in loadedData.dlls)
					{
						assemblyStaticIdMap.Add(assembly, modStaticID);
					}
				}
			}
		}

		var callingAssembly = Assembly.GetCallingAssembly();
		string modNamespace;
		if (assemblyStaticIdMap.TryGetValue(callingAssembly, out var staticId))
		{
			modNamespace = staticId;
		}
		else
		{
			Debug.LogWarning(
				$"[Twitch Integration] Unable to find a static ID for assembly {callingAssembly}, it will not be namespaced"
			);
			modNamespace = "";
		}

		id = $"{modNamespace}.{id}";
		registeredEvents.Add(id, new RefActionWrapper(delegate { }));
		idNameMap.Add(id, friendlyName);
		var eventInfo = new EventInfo(id);
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
