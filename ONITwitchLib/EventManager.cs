using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

public class EventManager
{
	// the EventManager instance from the event lib, but type erased as an object
	private readonly object eventManagerInstance;

	// delegates created to wrap various methods on the event manager without needing to use reflection
	// and Invoke every time
	private readonly Func<string, string, object> registerEventDelegate;
	private readonly Action<object, object> renameEventDelegate;
	private readonly Func<object, object> getFriendlyNameDelegate;
	private readonly Func<string, string, object> getEventByIdDelegate;
	private readonly Action<object, Action<string>> addListenerForEventDelegate;
	private readonly Action<object, Action<object>> removeListenerForEventDelegate;
	private readonly Action<object, object> triggerEventDelegate;

	internal EventManager(object instance)
	{
		eventManagerInstance = instance;
		var eventType = eventManagerInstance.GetType();
		var registerInfo = AccessTools.DeclaredMethod(
			eventType,
			"RegisterEvent",
			new[] { typeof(string), typeof(string) }
		);
		registerEventDelegate =
			DelegateUtil.CreateDelegate<Func<string, string, object>>(registerInfo, eventManagerInstance);
		var renameEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"RenameEvent",
			new[] { EventInterface.EventInfoType, typeof(string) }
		);
		renameEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			renameEventInfo,
			eventManagerInstance,
			EventInterface.EventInfoType,
			typeof(string)
		);
		var getNameInfo = AccessTools.DeclaredMethod(
			eventType,
			"GetFriendlyName",
			new[] { EventInterface.EventInfoType }
		);
		getFriendlyNameDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			getNameInfo,
			eventManagerInstance,
			EventInterface.EventInfoType,
			typeof(string)
		);
		var getByIdInfo = AccessTools.DeclaredMethod(
			eventType,
			"GetEventByID",
			new[] { typeof(string), typeof(string) }
		);
		getEventByIdDelegate =
			DelegateUtil.CreateDelegate<Func<string, string, object>>(getByIdInfo, eventManagerInstance);
		var addListenerForEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"AddListenerForEvent",
			new[] { EventInterface.EventInfoType, typeof(Action<object>) }
		);
		addListenerForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			addListenerForEventInfo,
			eventManagerInstance,
			EventInterface.EventInfoType,
			typeof(Action<object>)
		);
		var removeListenerForEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"RemoveListenerForEvent",
			new[] { EventInterface.EventInfoType, typeof(Action<object>) }
		);
		removeListenerForEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			removeListenerForEventInfo,
			eventManagerInstance,
			EventInterface.EventInfoType,
			typeof(Action<object>)
		);
		var triggerEventInfo = AccessTools.DeclaredMethod(
			eventType,
			"TriggerEvent",
			new[] { EventInterface.EventInfoType, typeof(object) }
		);
		triggerEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			triggerEventInfo,
			eventManagerInstance,
			EventInterface.EventInfoType,
			typeof(object)
		);
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
		var output = registerEventDelegate(id, friendlyName);
		if (output.GetType() != EventInterface.EventInfoType)
		{
			throw new Exception("event register type");
		}

		return new EventInfo(output);
	}

	/// <summary>
	/// Changes the user-facing name for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to be changed</param>
	/// <param name="friendlyName">The new name for the event</param>
	public void RenameEvent([NotNull] EventInfo eventInfo, [NotNull] string friendlyName)
	{
		renameEventDelegate(eventInfo.EventInfoInstance, friendlyName);
	}

	/// <summary>
	/// Gets the user-facing name for an event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event</param>
	/// <returns>The friendly name, if it exists, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public string GetFriendlyName([NotNull] EventInfo eventInfo)
	{
		return (string) getFriendlyNameDelegate(eventInfo.EventInfoInstance);
	}

	/// <summary>
	/// Gets an <see cref="EventInfo"/> for the specified ID, if the ID is registered.
	/// </summary>
	/// <param name="eventNamespace">The namespace for the ID</param>
	/// <param name="id">The ID to look for</param>
	/// <returns>An <see cref="EventInfo"/> representing the event, if the ID is found, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public EventInfo GetEventByID([NotNull] string eventNamespace, [NotNull] string id)
	{
		var output = getEventByIdDelegate(eventNamespace, id);
		if (output.GetType() != EventInterface.EventInfoType)
		{
			throw new Exception("event by id type");
		}

		return new EventInfo(output);
	}

	/// <summary>
	/// Adds a listener to the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to listen to</param>
	/// <param name="listener">The listener to call when the event is triggered</param>
	public void AddListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<object> listener)
	{
		addListenerForEventDelegate(eventInfo.EventInfoInstance, listener);
	}

	/// <summary>
	/// Removes a listener from the specified event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to remove from</param>
	/// <param name="listener">The listener to be removed from the event</param>
	/// <exception cref="ArgumentException"><paramref name="listener"/> was not listening to the event</exception>
	public void RemoveListenerForEvent([NotNull] EventInfo eventInfo, [NotNull] Action<object> listener)
	{
		removeListenerForEventDelegate(eventInfo.EventInfoInstance, listener);
	}

	/// <summary>
	/// Triggers an event with the passed data.  This calls all listeners of the event.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> for the event to trigger</param>
	/// <param name="data">The data to be passed to all listeners of the event</param>
	public void TriggerEvent([NotNull] EventInfo eventInfo, object data)
	{
		triggerEventDelegate(eventInfo.EventInfoInstance, data);
	}
}
