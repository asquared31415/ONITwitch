using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
///     Various constants for use across Twitch Integration and mods that depend on it.
/// </summary>
[PublicAPI]
public static class Consts
{
	/// <summary>
	///     Constants to use for event weights.  Using these constants is not mandatory, but is heavily suggested so that
	///     events have a consistent relative frequency.
	///     <br />
	///     Suggested usage is:
	///     <code>
	/// 		using static ONITwitchLib.Consts;
	/// 		// ...
	/// 		var myWeight = EventWeight.Common;
	///     </code>
	/// </summary>
	[PublicAPI]
	public static class EventWeight
	{
		/// <summary>
		///     Literally 0 weight.  Events with this weight will never be drawn.  This is mostly here for consistency in
		///     case someone finds it useful.
		/// </summary>
		[PublicAPI] public const int Never = 0;

		/// <summary>
		///     Events that should be possible, but should be exceptionally rare, things that should only want to happen to a
		///     colony once in hundreds or thousands of cycles.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Kill Duplicant</li>
		///         <li>Lava Flood</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int AlmostNever = 1;

		/// <summary>
		///     Events that are very rare, for example because they create a unique item or cause significant damage.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Lava Flood</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int VeryRare = 2;

		/// <summary>
		///     Events that should occur rarely.  Events in this category should not be colony ending if they are dangerous.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Uninsulate Tiles</li>
		///         <li>Morph Critters</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int Rare = 5;

		/// <summary>
		///     Events that should occur infrequently.  Events in this category should be a decent challenge for the player or
		///     provide something useful.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Tune Geyser</li>
		///         <li>Spawn Deadly Element (note that they are encased in a perfect insulator)</li>
		///         <li>Oil and Ethanol Flood</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int Uncommon = 10;

		/// <summary>
		///     Events that should occur commonly and regularly.  Events in this category should not cause significant harm to a
		///     colony.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Surprise Box</li>
		///         <li>Glitter Puft</li>
		///         <li>Sleepy Dupes</li>
		///         <li>Fart</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int Common = 20;

		/// <summary>
		///     Events that should occur very frequently.  Events in this category should be mostly neutral, neither providing much
		///     help nor causing much harm to a colony.
		///     <br />
		///     Examples of this weight in the base Twitch Integration mod are:
		///     <ul>
		///         <li>Spawn Solid/Liquid</li>
		///         <li>Spawn Dupe</li>
		///     </ul>
		/// </summary>
		[PublicAPI] public const int Frequent = 40;
	}
}
