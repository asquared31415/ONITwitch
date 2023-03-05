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

	public static class EVENTS
	{
		public static readonly LocString ATTRIBUTE_ATHLETICS_UP = "Athletics Up";
		public static readonly LocString ATTRIBUTE_ATHLETICS_DOWN = "Athletics Down";
		public static readonly LocString ATTRIBUTE_CONSTRUCTION_UP = "Construction Up";
		public static readonly LocString ATTRIBUTE_CONSTRUCTION_DOWN = "Construction Down";
		public static readonly LocString ATTRIBUTE_EXCAVATION_UP = "Excavation Up";
		public static readonly LocString ATTRIBUTE_EXCAVATION_DOWN = "Excavation Down";
		public static readonly LocString ATTRIBUTE_STRENGTH_UP = "Strength Up";
		public static readonly LocString ATTRIBUTE_STRENGTH_DOWN = "Strength Down";

		public static readonly LocString BANSHEE_WAIL = "Banshee Wail";

		public static readonly LocString BEDROOMS_SLIME = "Slimy Bedrooms";
		public static readonly LocString BEDROOM_SNOW = "Snowy Bedrooms";

		public static readonly LocString ECLIPSE = "Eclipse";

		public static readonly LocString ELEMENT_GROUP_COMMON = "Spawn Common Element";
		public static readonly LocString ELEMENT_GROUP_DEADLY = "Spawn Deadly Element";
		public static readonly LocString ELEMENT_GROUP_EXOTIC = "Spawn Exotic Element";
		public static readonly LocString ELEMENT_GROUP_GAS = "Spawn Gas";
		public static readonly LocString ELEMENT_GROUP_LIQUID = "Spawn Liquid";
		public static readonly LocString ELEMENT_GROUP_METAL = "Spawn Metal";

		public static readonly LocString FART = "Fart";

		public static readonly LocString FLOOD_ETHANOL = "Ethanol Flood";
		public static readonly LocString FLOOD_GOLD = "Gold Rain";
		public static readonly LocString FLOOD_WATER = "Water Flood";
		public static readonly LocString FLOOD_LAVA = "Lava Flood";
		public static readonly LocString FLOOD_NUCLEAR_WASTE = "Nuclear Waste Disposal Accident";
		public static readonly LocString FLOOD_OIL = "Oil Flood";
		public static readonly LocString FLOOD_POLLUTED_WATER = "Polluted Water Flood";

		public static readonly LocString GEYSER_MODIFICATION = "Tune Geyser";

		public static readonly LocString GLOBAL_WARMING = "Global Warming";
		public static readonly LocString ICE_AGE = "Ice Age";

		public static readonly LocString KILL_DUPE = "Kill Duplicant";
		public static readonly LocString MORPH_CRITTERS = "Morph Critters";
		public static readonly LocString PARTY_TIME = "Party Time";
		public static readonly LocString PEE = "Gold Rain";
		public static readonly LocString POCKET_DIMENSION = "Spawn Pocket Dimension";
		public static readonly LocString POISON = "Poison Duplicants";
		public static readonly LocString POOPSPLOSION = "Poopsplosion";

		public static readonly LocString RAIN_PREFAB_BEE = "Bee Infestation";
		public static readonly LocString RAIN_PREFAB_DIAMOND = "Diamond Rain";
		public static readonly LocString RAIN_PREFAB_GOLD = "Gold Rain";
		public static readonly LocString RAIN_PREFAB_MORB_AMORBUS = "Amorbus Rain";
		public static readonly LocString RAIN_PREFAB_MORB_NORMAL = "Morbs!!!";
		public static readonly LocString RAIN_PREFAB_PACU_NORMAL = "Pacu Rain";
		public static readonly LocString RAIN_PREFAB_PACU_PACUWU = "PacUwU Rain 🐠👉👈";
		public static readonly LocString RAIN_PREFAB_SLICKSTER = "Slickster Rain";

		public static readonly LocString REDUCE_OXYGEN = "Un-include Oxygen";
		public static readonly LocString RESEARCH_TECH = "Research Technology";
		public static readonly LocString SKILL_POINTS = "Skilling Spree";
		public static readonly LocString SLEEPY_DUPES = "Sleepy Dupes";
		public static readonly LocString SPICE_FOOD = "Spice Food";

		public static readonly LocString SPAWN_ATMO_SUIT = "Spawn Atmo Suit";
		public static readonly LocString SPAWN_CRAB = "Spawn Pokeshell";
		public static readonly LocString SPAWN_DUPE = "Spawn Duplicant";
		public static readonly LocString SPAWN_GLITTER_PUFT = "Spawn Glitter Puft";
		public static readonly LocString SPAWN_MOO_COMET = "Spawn Gassy Moo Comet";
		public static readonly LocString SPAWN_SNAZZY_SUIT = "Spawn Snazzy Suit";
		public static readonly LocString SPAWN_VACILLATOR_CHARGE = "Spawn Vacillator Charge";

		public static readonly LocString STRESS_ADD = "Add Stress";
		public static readonly LocString STRESS_REMOVE = "Remove Stress";

		public static readonly LocString SURPRISE = "Surprise!!!";
		public static readonly LocString SURPRISE_BOX = "Surprise Box";

		public static readonly LocString TILE_TEMP_DOWN = "Icy Floors";
		public static readonly LocString TILE_TEMP_UP = "The Floor is Lava";

		public static readonly LocString UNINSULATE = "Uninsulate Tiles";
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

		public static class ELEMENT_GROUP
		{
			public static readonly LocString TITLE = "Element Created";
			public static readonly LocString BODY_FORMAT = "A chunk of {0} has appeared";
		}

		public static class END_VOTE_NO_OPTIONS
		{
			public static readonly LocString TITLE = "Vote Complete";
			public static readonly LocString BODY = "No events were voted for";
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

		public static class FLOOD
		{
			public static readonly LocString TITLE = "Flood";
			public static readonly LocString BODY_FORMAT = "A flood of {0} was spawned";
		}

		public static class GEYSER_MODIFICATION
		{
			public static readonly LocString TITLE = "Geyser Modified";
			public static readonly LocString BODY_FORMAT = "Geyser {0} is temporarily tuned";
		}

		public static class GLOBAL_WARMING
		{
			public static readonly LocString TITLE = "Global Warming";
			public static readonly LocString BODY_FORMAT = "Temperatures on {0} are on the rise";
		}

		public static class ICE_AGE
		{
			public static readonly LocString TITLE = "Ice Age";
			public static readonly LocString BODY_FORMAT = "All liquids on {0} have frozen";
		}

		public static class KILL_DUPE
		{
			public static readonly LocString TITLE = "Duplicant Killed";
			public static readonly LocString BODY_FORMAT = "{0} died :(";
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

		public static class POCKET_DIMENSION
		{
			public static readonly LocString TITLE = "Pocket Dimension";
			public static readonly LocString BODY = "A portal to a pocket dimension opened";
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

		public static class SPAWN_DUPE
		{
			public static readonly LocString TITLE = "Spawning Duplicant";
			public static readonly LocString BODY_FORMAT = "{0} was brought into the world";
		}

		public static class SPAWN_PREFAB
		{
			public static readonly LocString TITLE = "Spawning Object";
			public static readonly LocString BODY_FORMAT = "A new {0} was created";
		}

		public static class SPICE_FOOD
		{
			public static readonly LocString TITLE = "Spiced Food";
			public static readonly LocString BODY = "All food has had spices applied";
		}

		public static class STARTING_VOTE
		{
			public static readonly LocString TITLE = "Starting Vote";
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

		public static class SURPRISE_BOX
		{
			public static readonly LocString TITLE = "Surprise Box";
			public static readonly LocString BODY = "I wonder what is inside?";
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
