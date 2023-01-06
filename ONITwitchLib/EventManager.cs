using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

public class EventManager
{
	/// <summary>
	/// Gets the instance of the event manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static EventManager Instance => instance ??= GetEventManagerInstance();

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

	private static EventManager instance;

	private static Func<object> eventManagerInstanceDelegate;

	private static EventManager GetEventManagerInstance()
	{
		if (eventManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(EventInterface.EventManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != EventInterface.EventManagerType)
			{
				throw new Exception(
					$"The Instance property on {EventInterface.EventManagerType.AssemblyQualifiedName} does not return an instance of {EventInterface.EventManagerType}"
				);
			}

			// no argument because it's static property
			eventManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		return new EventManager(eventManagerInstanceDelegate());
	}

	// delegates created to wrap various methods on the event manager without needing to use reflection
	// and Invoke every time
	private readonly Func<string, string, object> getEventByIdDelegate;

	internal EventManager(object instance)
	{
		var eventType = instance.GetType();
		var getByIdInfo = AccessTools.DeclaredMethod(
			eventType,
			"GetEventByID",
			new[] { typeof(string), typeof(string) }
		);
		getEventByIdDelegate =
			DelegateUtil.CreateDelegate<Func<string, string, object>>(getByIdInfo, instance);
	}
}
