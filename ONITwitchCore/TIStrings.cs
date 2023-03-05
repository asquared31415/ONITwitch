// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using STRINGS;

#pragma warning disable CS1591
namespace ONITwitchCore;

public static class STRINGS
{
	public static class TESTING
	{
		public static readonly LocString TEXT = "Testing text";
	}

	public static class BUILDINGS
	{
		public static class PREFABS
		{
			public static class ONITWITCH
			{
				public static class POCKETDIMENSIONEXTERIORPORTALCONFIG
				{
					public static readonly LocString NAME = "Pocket Dimension Portal";
					public static readonly LocString DESC = "TODO";
					public static readonly LocString EFFECT = "A portal to an unknown dimension.";
				}

				public static class POCKETDIMENSIONINTERIORPORTALCONFIG
				{
					public static readonly LocString NAME = "Return Portal";
					public static readonly LocString DESC = "TODO";

					public static readonly LocString EFFECT
						= "A portal that should hopefully return your duplicants back where they came from.";
				}
			}
		}
	}

	public static class CREATURES
	{
		public static class SPECIES
		{
			public static class GLITTERPUFT
			{
				public static readonly LocString NAME = "Glitter Puft";

				public static readonly LocString DESC = "A extraordinarily colorful and very pretty Puft";
			}
		}
	}

	public static class ELEMENTS
	{
		public static class INDESTRUCTIBLEELEMENT
		{
			public static readonly LocString NAME = UI.FormatAsLink(
				"Neutrollium",
				nameof(global::STRINGS.ELEMENTS.UNOBTANIUM)
			);

			public static readonly LocString DESC = global::STRINGS.ELEMENTS.UNOBTANIUM.DESC;
		}
	}

	public static class ITEMS
	{
		public static class ONITWITCH
		{
			public static class SURPRISEBOXCONFIG
			{
				public static readonly LocString NAME = "Surprise Box";
				public static readonly LocString EFFECT = "Deconstruct for a surprise!";
				public static readonly LocString DESC = "";
			}
		}

		public static class FOOD
		{
			public static class ONITWITCH
			{
				public static class GLITTERMEATCONFIG
				{
					public static readonly LocString NAME = "Glitter Meat";

					public static readonly LocString DESC =
						"A piece of meat that glitters faintly.\n\n...It's best not to think about where it came from...";
				}
			}
		}
	}

	public static class TOASTS
	{
		public static class BANSHEE_WAIL
		{
			public static readonly LocString TITLE = "Banshee Wail";
			public static readonly LocString BODY = "All of your duplicants are releasing their urge to scream";
		}

		public static class ECLIPSE
		{
			public static readonly LocString TITLE = "Eclipse";
			public static readonly LocString BODY = "A long darkness has enveloped the colony";
		}

		public static class EFFECT
		{
			public static readonly LocString TITLE = "Effect Applied";
			public static readonly LocString BODY_FORMAT = "All dupes have had the {0} effect applied";
		}

		public static class FART
		{
			public static readonly LocString TITLE = "Fart";

			public static readonly LocString
				BODY = "All of your duplicants have farted. What was in that Frost Burger?";
		}

		public static class FILL_BEDROOMS
		{
			public static readonly LocString TITLE = "Bedrooms Bombed";
			public static readonly LocString BODY_FORMAT = "{0} was placed in every bedroom";
		}

		public static class PARTY_TIME
		{
			public static readonly LocString TITLE = "Party Time!";
			public static readonly LocString BODY = "The dupes are throwing a rainbow party!";
		}

		public static class PEE
		{
			public static readonly LocString TITLE = "Pee Everywhere";
			public static readonly LocString BODY = "All duplicants suddenly feel the need to relieve themselves";
		}

		public static class POISON
		{
			public static readonly LocString TITLE = "Poisoned Dupes";
			public static readonly LocString BODY = "All duplicants are poisoned and will take damage over time";
		}

		public static class POOPSPLOSION
		{
			public static readonly LocString TITLE = "Poopsplosion";
			public static readonly LocString BODY = "All toilets have exploded with poop";
		}

		public static class RAIN_PREFAB
		{
			public static readonly LocString TITLE = "Rain";
			public static readonly LocString BODY_FORMAT = "A rain of {0} is starting";
		}

		public static class REDUCE_OXYGEN
		{
			public static readonly LocString TITLE = "Oxygen Reduced";
			public static readonly LocString BODY_FORMAT = "The mass of all breathable elements was reduced by {0}%";
		}

		public static class RESEARCH_TECH
		{
			public static readonly LocString TITLE = "Technology Researched";
			public static readonly LocString BODY_FORMAT = "{0} has been researched";
		}

		public static class SKILLS
		{
			public static readonly LocString TITLE = "Skilling Spree";

			public static readonly LocString BODY =
				"Some duplicants have been inspired and instantly gained a skill point";
		}

		public static class SLEEPY_DUPES
		{
			public static readonly LocString TITLE = "Sleepy Dupes";
			public static readonly LocString BODY = "All duplicants are extremely exhausted";
		}

		public static class SPICE_FOOD
		{
			public static readonly LocString TITLE = "Spiced Food";
			public static readonly LocString BODY = "All food has had spices applied";
		}

		public static class STRESS_INCREASE
		{
			public static readonly LocString TITLE = "Stress Wave";
			public static readonly LocString BODY = "All duplicants are very stressed about life";
		}

		public static class STRESS_DECREASE
		{
			public static readonly LocString TITLE = "Calming Wave";
			public static readonly LocString BODY = "All duplicants are less stressed";
		}

		public static class TILE_TEMP_UP
		{
			public static readonly LocString TITLE = "The Floor is Lava";
			public static readonly LocString BODY = "All non-natural tiles are warmer";
		}

		public static class TILE_TEMP_DOWN
		{
			public static readonly LocString TITLE = "Icy Floors";
			public static readonly LocString BODY = "All non-natural tiles are colder";
		}

		public static class UNINSULATE_TILES
		{
			public static readonly LocString TITLE = "Tiles Uninsulated";
			public static readonly LocString BODY = "Many tiles have been replaced with non-insulated versions";
		}
	}
}
