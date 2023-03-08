using Klei.AI;

namespace ONITwitchCore.Content;

public static class CustomEffects
{
	public static readonly Effect AthleticsUpEffect = new(
		"ONITwitch.AttributeAthleticsUp",
		STRINGS.DUPLICANTS.MODIFIERS.ATHLETICS_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ATHLETICS_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect AthleticsDownEffect = new(
		"ONITwitch.AttributeAthleticsDown",
		STRINGS.DUPLICANTS.MODIFIERS.ATHLETICS_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.ATHLETICS_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect ConstructionUpEffect = new(
		"ONITwitch.AttributeConstructionUp",
		STRINGS.DUPLICANTS.MODIFIERS.CONSTRUCTION_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.CONSTRUCTION_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect ConstructionDownEffect = new(
		"ONITwitch.AttributeConstructionDown",
		STRINGS.DUPLICANTS.MODIFIERS.CONSTRUCTION_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.CONSTRUCTION_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect ExcavationUpEffect = new(
		"ONITwitch.AttributeExcavationUp",
		STRINGS.DUPLICANTS.MODIFIERS.EXCAVATION_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.EXCAVATION_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect ExcavationDownEffect = new(
		"ONITwitch.AttributeExcavationDown",
		STRINGS.DUPLICANTS.MODIFIERS.EXCAVATION_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.EXCAVATION_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect StrengthUpEffect = new(
		"ONITwitch.AttributeStrengthUp",
		STRINGS.DUPLICANTS.MODIFIERS.STRENGTH_UP.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.STRENGTH_UP.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect StrengthDownEffect = new(
		"ONITwitch.AttributeStrengthDown",
		STRINGS.DUPLICANTS.MODIFIERS.STRENGTH_DOWN.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.STRENGTH_DOWN.TOOLTIP,
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect SleepyEffect = new(
		"ONITwitch.SleepyDupesEffect",
		STRINGS.DUPLICANTS.MODIFIERS.SLEEPY.NAME,
		STRINGS.DUPLICANTS.MODIFIERS.SLEEPY.TOOLTIP,
		0.5f * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static void SetupEffects()
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
		SleepyEffect.Add(new AttributeModifier(amounts.Stamina.deltaAttribute.Id, -10f, STRINGS.DUPLICANTS.MODIFIERS.SLEEPY.TOOLTIP));
		effects.Add(SleepyEffect);
	}
}
