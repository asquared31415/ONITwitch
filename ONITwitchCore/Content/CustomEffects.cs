using Klei.AI;

namespace ONITwitchCore.Content;

public static class CustomEffects
{
	public static readonly Effect AthleticsUpEffect = new(
		"ONITwitch.AttributeAthleticsUp",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect AthleticsDownEffect = new(
		"ONITwitch.AttributeAthleticsDown",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect ConstructionUpEffect = new(
		"ONITwitch.AttributeConstructionUp",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect ConstructionDownEffect = new(
		"ONITwitch.AttributeConstructionDown",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect ExcavationUpEffect = new(
		"ONITwitch.AttributeExcavationUp",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect ExcavationDownEffect = new(
		"ONITwitch.AttributeExcavationDown",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static readonly Effect StrengthUpEffect = new(
		"ONITwitch.AttributeStrengthUp",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		false
	);

	public static readonly Effect StrengthDownEffect = new(
		"ONITwitch.AttributeStrengthDown",
		"TODO",
		"TODO",
		2 * Constants.SECONDS_PER_CYCLE,
		true,
		true,
		true
	);

	public static void SetupEffects()
	{
		var effects = Db.Get().effects;
		var attributes = Db.Get().Attributes;
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
	}
}