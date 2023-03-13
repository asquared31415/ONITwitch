using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// Represents how much harm an <see cref="EventInfo"/> can do.
/// This estimate should be a reasonable maximum and should take into account both short-term and long-term consequences.
/// </summary>
[PublicAPI]
public enum Danger
{
	/// <summary>
	/// The event does not cause any damage. 
	/// </summary>
	None = 0,

	/// <summary>
	/// The event causes only minor damage that can be easily resolved.
	/// </summary>
	Small = 1,

	/// <summary>
	/// The event causes moderate damage, with possible long-term effects, but does not pose a significant threat to survival.
	/// </summary>
	Medium = 2,

	/// <summary>
	/// The event causes significant damage, often with significant long-term effects. Recovery is possible, but requires planning and knowledge.
	/// </summary>
	High = 3,

	/// <summary>
	/// The event is extremely dangerous. In some cases dupes may directly die. Long-term effects can alter the course of the colony. 
	/// </summary>
	Extreme = 4,

	/// <summary>
	/// This event will cause deaths and have significant long-term effects. Preventing or repairing damage done may not be possible.
	/// </summary>
	/// <remarks>
	/// Examples of <see cref="Deadly"/> events are directly killing a duplicant or spawning large quantities of magma.
	/// This danger level is not enabled by default and players are warned that it is exceptionally dangerous before selecting it.
	/// </remarks>
	Deadly = 5,
}
