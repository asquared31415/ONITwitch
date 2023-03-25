using JetBrains.Annotations;

namespace ONITwitch.CustomElements;

/// <summary>
/// SimHashes for the Twitch mod.
/// </summary>
[PublicAPI]
public static class TwitchSimHashes
{
	/// <summary>
	/// The hash for the indestructible copy of Neutronium that exists to try and bypass mods
	/// that let users destroy Neutronium.
	/// </summary>
	[PublicAPI]
	public static readonly SimHashes IndestructibleElement = (SimHashes) Hash.SDBMLower(nameof(IndestructibleElement));
}
