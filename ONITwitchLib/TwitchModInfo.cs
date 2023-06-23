using System;
using System.IO;
using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// Various information about the Twitch mod, primarily for integrations.
/// </summary>
[PublicAPI]
public static class TwitchModInfo
{
	/// <summary>
	/// The prefix that the twitch mod uses for IDs, with a few exceptions
	/// </summary>
	[PublicAPI] public const string ModPrefix = "ONITwitch.";

	/// <summary>
	/// The static ID of the Twitch mod.
	/// </summary>
	[PublicAPI] public const string StaticID = "asquared31415.TwitchIntegration";

	/// <summary>
	/// True if the Twitch mod has been detected, false otherwise.
	/// Safe to access even if the Twitch mod is not installed or active.
	/// </summary>
	[PublicAPI]
	public static bool TwitchIsPresent => MainTwitchModType != null;

	/// <summary>
	/// The Type for the main Twitch mod's UserMod2, if it exists. null if it cannot be found.
	/// Safe to access even if the Twitch mod is not installed or active. 
	/// </summary>
	[PublicAPI]
	[CanBeNull]
	public static Type MainTwitchModType => mainTwitchType ??= Type.GetType(TwitchTypeName);

	/// <summary>
	/// The mod folder containing the Twitch mod dll.
	/// Only valid if the Twitch mod is active.
	/// </summary>
	[PublicAPI] public static readonly string MainModFolder =
		TwitchIsPresent ? Directory.GetParent(MainTwitchModType!.Assembly.Location)!.FullName : "";


	/// <summary>
	/// The folder that holds the config for the mod.
	/// Only valid if the Twitch mod is active.
	/// </summary>
	[PublicAPI] public static readonly string ConfigFolder =
		// mods/steam/ID/dll -> mods/config/ID
		// 3 directories up, back down to config/id
		Path.Combine(
			Directory.GetParent(MainTwitchModType!.Assembly.Location)!.Parent!.Parent!.FullName,
			"config",
			StaticID
		);

	/// <summary>
	/// The filename of the config file containing the main Twitch mod config.
	/// </summary>
	[PublicAPI] public const string ConfigName = "config.json";

	/// <summary>
	/// The path to the config file containing the main Twitch mod config.
	/// Only valid if the Twitch mod is active.
	/// </summary>
	[PublicAPI] public static readonly string ConfigPath = Path.Combine(ConfigFolder, ConfigName);

	private const string TwitchTypeName = "ONITwitch.OniTwitchMod, ONITwitch";
	private static Type mainTwitchType;
}
