using JetBrains.Annotations;
using Klei.AI;
using ONITwitchLib;

namespace ONITwitchCore.Content;

/// <summary>
/// Custom effects that the core mod uses.
/// </summary>
[PublicAPI]
public static class CustomEffects
{
	/// <summary>
	/// An effect that increases a duplicant's Athletics by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect AthleticsUpEffect = new(
		TwitchModInfo.ModPrefix + "AttributeAthleticsUp",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.ATHLETICS_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.ATHLETICS_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	/// <summary>
	/// An effect that decreases a duplicant's Athletics by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect AthleticsDownEffect = new(
		TwitchModInfo.ModPrefix + "AttributeAthleticsDown",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.ATHLETICS_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.ATHLETICS_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	/// <summary>
	/// An effect that increases a duplicant's Construction by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect ConstructionUpEffect = new(
		TwitchModInfo.ModPrefix + "AttributeConstructionUp",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.CONSTRUCTION_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.CONSTRUCTION_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	/// <summary>
	/// An effect that decreases a duplicant's Construction by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect ConstructionDownEffect = new(
		TwitchModInfo.ModPrefix + "AttributeConstructionDown",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.CONSTRUCTION_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.CONSTRUCTION_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	/// <summary>
	/// An effect that increases a duplicant's Excavation by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect ExcavationUpEffect = new(
		TwitchModInfo.ModPrefix + "AttributeExcavationUp",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.EXCAVATION_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.EXCAVATION_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	/// <summary>
	/// An effect that decreases a duplicant's Excavation by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect ExcavationDownEffect = new(
		TwitchModInfo.ModPrefix + "AttributeExcavationDown",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.EXCAVATION_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.EXCAVATION_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	/// <summary>
	/// An effect that increases a duplicant's Strength by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect StrengthUpEffect = new(
		TwitchModInfo.ModPrefix + "AttributeStrengthUp",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.STRENGTH_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.STRENGTH_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	/// <summary>
	/// An effect that decreases a duplicant's Strength by 5 for 2 cycles.
	/// </summary>
	[PublicAPI] public static readonly Effect StrengthDownEffect = new(
		TwitchModInfo.ModPrefix + "AttributeStrengthDown",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.STRENGTH_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.STRENGTH_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	/// <summary>
	/// An effect that makes a duplicant's stamina decrease rapidly, faster than sleep can recover.
	/// </summary>
	[PublicAPI] public static readonly Effect SleepyEffect = new(
		TwitchModInfo.ModPrefix + "SleepyDupesEffect",
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.SLEEPY.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.SLEEPY.TOOLTIP,
		0.5f * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	internal static void SetupEffects()
	{
		var effects = Db.Get().effects;
		var attributes = Db.Get().Attributes;
		var amounts = Db.Get().Amounts;
		AthleticsUpEffect.Add(new AttributeModifier(attributes.Athletics.Id, 5));
		effects.Add(AthleticsUpEffect);
		AthleticsDownEffect.Add(new AttributeModifier(attributes.Athletics.Id, -5));
		effects.Add(AthleticsDownEffect);
		ConstructionUpEffect.Add(new AttributeModifier(attributes.Construction.Id, 5));
		effects.Add(ConstructionUpEffect);
		ConstructionDownEffect.Add(new AttributeModifier(attributes.Construction.Id, -5));
		effects.Add(ConstructionDownEffect);
		ExcavationUpEffect.Add(new AttributeModifier(attributes.Digging.Id, 5));
		effects.Add(ExcavationUpEffect);
		ExcavationDownEffect.Add(new AttributeModifier(attributes.Digging.Id, -5));
		effects.Add(ExcavationDownEffect);
		StrengthUpEffect.Add(new AttributeModifier(attributes.Strength.Id, 5));
		effects.Add(StrengthUpEffect);
		StrengthDownEffect.Add(new AttributeModifier(attributes.Strength.Id, -5));
		effects.Add(StrengthDownEffect);
		SleepyEffect.Add(
			new AttributeModifier(
				amounts.Stamina.deltaAttribute.Id,
				-10f,
				STRINGS.DUPLICANTS.MODIFIERS.ONITWITCH.SLEEPY.TOOLTIP
			)
		);
		effects.Add(SleepyEffect);
	}
}
