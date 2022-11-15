using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public static class EventInterface
{
	private const string TwitchTypeName = "ONITwitch.OniTwitchMod, ONITwitch";
	private static Type mainTwitchType;

	private const string EventManagerTypeName = "EventLib.EventManager, ONITwitch";
	private static Type eventManagerType;

	private const string EventInfoTypeName = "EventLib.EventInfo, ONITwitch";
	private static Type eventInfoType;

	/// <summary>
	/// The Type for the main Twitch mod's UserMod2, if it exists. null if it cannot be found.
	/// Safe to access even if the Twitch mod is not installed or active. 
	/// </summary>
	[CanBeNull]
	public static Type MainTwitchModType => mainTwitchType ??= Type.GetType(TwitchTypeName);

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventManagerType => (eventManagerType ??= Type.GetType(EventManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventInfoType => (eventInfoType ??= Type.GetType(EventInfoTypeName))!;

	/// <summary>
	/// True if the Twitch mod has been detected, false otherwise.
	/// Safe to access even if the Twitch mod is not installed or active.
	/// </summary>
	public static bool TwitchIsPresent => MainTwitchModType != null;

	private static Func<object> eventManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the event manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static EventManager GetEventManagerInstance()
	{
		if (eventManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(EventManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != EventManagerType)
			{
				throw new Exception(
					$"The Instance property on {EventManagerType.AssemblyQualifiedName} does not return an instance of {EventManagerType}"
				);
			}

			// no argument because it's static property
			eventManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = eventManagerInstanceDelegate();
		return new EventManager(instance);
	}
}
