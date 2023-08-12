using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

/// <summary>
///     Provides access to settings from the main Twitch Integration mod.
/// </summary>
[PublicAPI]
public static class TwitchSettings
{
	private static Func<IReadOnlyDictionary<string, object>> getSettingsDictionaryFunc =
		DelegateUtil.CreateDelegate<Func<IReadOnlyDictionary<string, object>>>(
			AccessTools.Method(CoreTypes.TwitchSettingsType, "GetSettingsData", Type.EmptyTypes),
			null
		);

	/// <summary>
	///     Gets all of the settings from Twitch Integration.
	///     This is a copy of the settings, it is not possible to modify the settings by modifying this data.
	/// </summary>
	/// <returns>All of the settings, in a Dictionary.</returns>
	/// <remarks>
	///     Note that keys and values are <b>not</b> stable. Keys may be added or removed between Twitch Integration
	///     updates. Handle a missing setting appropriately.
	/// </remarks>
	[PublicAPI]
	[NotNull]
	public static IReadOnlyDictionary<string, object> GetSettingsDictionary()
	{
		return getSettingsDictionaryFunc();
	}
}
