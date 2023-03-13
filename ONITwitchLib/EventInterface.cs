using System;
using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// The main interface to the Twitch mod.
/// </summary>
public static class EventInterface
{
	private const string EventManagerTypeName = "EventLib.EventManager, ONITwitch";
	private static Type eventManagerType;

	private const string EventInfoTypeName = "EventLib.EventInfo, ONITwitch";
	private static Type eventInfoType;

	private const string DataManagerTypeName = "EventLib.DataManager, ONITwitch";
	private static Type dataManagerType;

	private const string TwitchDeckManagerTypeName = "ONITwitchCore.TwitchDeckManager, ONITwitch";
	private static Type twitchDeckManagerType;

	private const string EventGroupTypeName = "EventLib.EventGroup, ONITwitch";
	private static Type eventGroupType;

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
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type DataManagerType => (dataManagerType ??= Type.GetType(DataManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type TwitchDeckManagerType => (twitchDeckManagerType ??= Type.GetType(TwitchDeckManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventGroupType => (eventGroupType ??= Type.GetType(EventGroupTypeName))!;
}
