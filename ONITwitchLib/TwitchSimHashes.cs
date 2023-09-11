using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
///     SimHashes for the Twitch mod.
/// </summary>
[PublicAPI]
public static class TwitchSimHashes
{
	/// <summary>
	///     The hash for the indestructible copy of Neutronium that exists to try and bypass mods
	///     that let users destroy Neutronium.
	/// </summary>
	[PublicAPI] public static readonly SimHashes OniTwitchIndestructibleElement =
		(SimHashes) Hash.SDBMLower(nameof(OniTwitchIndestructibleElement));

	/// <summary>
	///     The hash for the perfect insulation element that Twitch Integration uses to withstand high pressure and very hot
	///     liquids.
	/// </summary>
	[PublicAPI] public static readonly SimHashes OniTwitchSuperInsulator =
		(SimHashes) Hash.SDBMLower(nameof(OniTwitchSuperInsulator));
}
