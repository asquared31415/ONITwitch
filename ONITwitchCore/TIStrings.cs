// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable IdentifierTypo

#pragma warning disable CS1591
#pragma warning disable CS0414

using ONITwitchLib.Utils;
using STRINGS;

namespace ONITwitch;

internal static class STRINGS
{
	public static class BUILDINGS
	{
		public static class PREFABS
		{
			public static class ONITWITCH
			{
				public static class POCKETDIMENSIONEXTERIORPORTALCONFIG
				{
					public static readonly LocString NAME = "Pocket Dimension Portal";
					public static readonly LocString DESC = "";
					public static readonly LocString EFFECT = "A portal to an unknown dimension.";
				}

				public static class POCKETDIMENSIONINTERIORPORTALCONFIG
				{
					public static readonly LocString NAME = "Return Portal";
					public static readonly LocString DESC = "";

					public static readonly LocString EFFECT =
						"A portal that returns your duplicants back where they came from.";
				}
			}
		}
	}

	public static class CREATURES
	{
		public static class SPECIES
		{
			public static class ONITWITCH
			{
				public static class GLITTERPUFT
				{
					public static readonly LocString NAME = "Glitter Puft";

					public static readonly LocString DESC = "A extraordinarily colorful and very pretty Puft";
				}
			}
		}
	}

	public static class DUPLICANTS
	{
		public static class MODIFIERS
		{
			public static class ONITWITCH
			{
				public static class ATHLETICS_UP
				{
					public static readonly LocString NAME = "Athletics Up";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class ATHLETICS_DOWN
				{
					public static readonly LocString NAME = "Athletics Down";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class CONSTRUCTION_UP
				{
					public static readonly LocString NAME = "Construction Up";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class CONSTRUCTION_DOWN
				{
					public static readonly LocString NAME = "Construction Down";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class EXCAVATION_UP
				{
					public static readonly LocString NAME = "Excavation Up";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class EXCAVATION_DOWN
				{
					public static readonly LocString NAME = "Excavation Down";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class SLEEPY
				{
					public static readonly LocString NAME = "Incredibly Sleepy";
					public static readonly LocString TOOLTIP = "Incredibly Exhausted";
				}

				public static class STRENGTH_UP
				{
					public static readonly LocString NAME = "Strength Up";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}

				public static class STRENGTH_DOWN
				{
					public static readonly LocString NAME = "Strength Down";
					public static readonly LocString TOOLTIP = "Caused by Twitch Chat";
				}
			}
		}
	}

	public static class ELEMENTS
	{
		public static class ONITWITCHINDESTRUCTIBLEELEMENT
		{
			public static readonly LocString NAME = UI.FormatAsLink(
				"Neutrollium",
				nameof(global::STRINGS.ELEMENTS.UNOBTANIUM)
			);

			public static readonly LocString DESC = global::STRINGS.ELEMENTS.UNOBTANIUM.DESC;
		}

		public static class ONITWITCHSUPERINSULATOR
		{
			public static readonly LocString NAME = "Insulation";

			public static readonly LocString DESC =
				"Some insulation that prevents its contents from immediately destroying your base. Don't worry, you don't get any resources out of this.";
		}
	}

	public static class ITEMS
	{
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

		public static class ONITWITCH
		{
			public static class SURPRISEBOXCONFIG
			{
				public static readonly LocString NAME = "Surprise Box";
				public static readonly LocString EFFECT = "Deconstruct for a surprise!";
				public static readonly LocString DESC = "";
			}

			public static class DEV_POCKET_DIMENSION_GENERATOR
			{
				public static readonly LocString NAME = "Pocket Dimension Generator";
				public static readonly LocString DESC = "Generates a Pocket Dimension";
			}
		}
	}

	public static class MISC
	{
		public static class STATUSITEMS
		{
			public static class ONITWITCH
			{
				public static class POISONED
				{
					public static LocString NAME = "Poisoned";
					public static LocString TOOLTIP = "This duplicant is poisoned for {time}";
				}

				public static class GEYSERTEMPORARILYTUNED
				{
					public static LocString NAME = "Geyser Temporarily Tuned";
					public static LocString TOOLTIP = "This geyser is being temporarily modified for {time}";
				}
			}
		}
	}

	public static class ONITWITCH
	{
		public static class DIAGNOSTICS
		{
			public static class DIMENSION_CLOSING
			{
				// idk where this is used
				public static readonly LocString CRITERION_NAME = "Pocket Dimension closing soon";

				public static readonly LocString DIAGNOSTIC_NAME = "Time Remaining";

				// this is not shown under normal cases
				public static readonly LocString ERROR = "Error";
				public static readonly LocString NORMAL = "Dimension Normal";
				public static readonly LocString CLOSING = "Low Time Remaining";
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
			public static readonly LocString SPAWN_MOO_COMET = "Spawn Gassy Mooteor";
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
				public static readonly LocString BODY_FORMAT = "{0} has appeared";
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

			public static class MORPH
			{
				public static readonly LocString TITLE = "Critters Morphed";
				public static readonly LocString BODY = "All critters have been morphed";
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

				public static readonly LocString
					BODY_FORMAT = "The mass of all breathable elements was reduced by {0}%";
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
				public static readonly LocString BODY = "All constructed tiles are warmer";
			}

			public static class TILE_TEMP_DOWN
			{
				public static readonly LocString TITLE = "Icy Floors";
				public static readonly LocString BODY = "All constructed tiles are colder";
			}

			public static class UNINSULATE_TILES
			{
				public static readonly LocString TITLE = "Tiles Uninsulated";
				public static readonly LocString BODY = "Many tiles have been replaced with non-insulated versions";
			}

			public static class WARNINGS
			{
				public static readonly LocString EVENT_FAILURE = "Event Failed";

				public static class SPAWN_DUPE_FAILURE
				{
					public static readonly LocString BODY =
						"Unable to find a position to spawn a duplicant at. " +
						"This normally means that no printing pods or other dupes exist. " +
						"If you believe this is incorrect, please file a bug report.";
				}
			}
		}

		public static class UI
		{
			public static class APPLY
			{
				public static readonly LocString TEXT = "Apply";
			}

			public static class CONFIG
			{
				public static readonly LocString TITLE = "Twitch Integration Settings";

				public static class CHANNEL_NAME
				{
					public static readonly LocString TEXT = "Channel Name";

					public static readonly LocString TOOLTIP =
						"The name of the Twitch channel to interact with. This should be your username. (example: asquared31415)";
				}

				public static class TIME_BETWEEN_VOTES
				{
					public static readonly LocString TEXT = "Time Between Votes";

					public static readonly LocString TOOLTIP =
						"The amount of real time, in seconds, that should pass between the end of one vote and the start of the next. (example: 600)";
				}

				public static class VOTING_TIME
				{
					public static readonly LocString TEXT = "Voting Time";

					public static readonly LocString TOOLTIP =
						"The amount of real time, in seconds, that a vote should be active for. (example: 60)";
				}

				public static class OPTIONS_PER_VOTE
				{
					public static readonly LocString TEXT = "Options per Vote (1-5)";

					public static readonly LocString TOOLTIP =
						"The number of options that should be available to vote for.";
				}

				public static class USE_TWITCH_COLORS
				{
					public static readonly LocString TEXT = "Use Twitch Username Colors";

					public static readonly LocString TOOLTIP =
						"Whether to copy a chatter's username color to their duplicant's name, if they are spawned. Note: If a user has not set their username color manually, the chosen color may be different than the color you see.";
				}

				public static class SHOW_TOASTS
				{
					public static readonly LocString TEXT = "Show Notifications";

					public static readonly LocString TOOLTIP =
						"Whether to show any notifications from the Twitch Integration mod or any events.";
				}

				public static class SHOW_START_TOASTS
				{
					public static readonly LocString TEXT = "Show Vote Choice Notifications";

					public static readonly LocString TOOLTIP =
						"Whether to show notifications when a vote starts. This can be disabled if you want to not know what options chat has.";
				}

				public static class MIN_DANGER
				{
					public static readonly LocString TEXT = "Min Danger";

					public static readonly LocString TOOLTIP =
						"Events with a danger less than this setting will not appear.";
				}

				public static class MAX_DANGER
				{
					public static readonly LocString TEXT = "Max Danger";

					public static readonly LocString TOOLTIP =
						"Events with a danger greater than this setting will not appear.";
				}

				public static class PHOTOSENSITIVE_MODE
				{
					public static readonly LocString TEXT = "Light Sensitivity Mode";

					public static readonly LocString TOOLTIP =
						"If this setting is enabled, events that use rapidly changing colors or lights will not occur.";
				}

				public static class EDIT
				{
					public static readonly LocString TEXT = "Edit Event Config";

					public static readonly LocString TOOLTIP =
						"Opens a page in a browser to edit the current event config. (WARNING: SPOILERS! All events are shown.)";
				}
			}

			public static class DANGER
			{
				public static readonly LocString NONE = "None";
				public static readonly LocString SMALL = "Small";
				public static readonly LocString MEDIUM = "Medium";
				public static readonly LocString HIGH = "High";
				public static readonly LocString EXTREME = "Extreme";
				public static readonly LocString DEADLY = "Deadly";
			}

			public static class DIALOGS
			{
				public static class CONNECTION_ERROR
				{
					public static readonly LocString TITLE = "Connection Error";

					public static readonly LocString BODY_FORMAT =
						"The connection to Twitch failed {0} times. This may be caused by a bad internet connection or invalid credentials.";
				}

				public static class DEADLY_CONFIG
				{
					public static readonly LocString TITLE = "Deadly Settings";

					public static readonly LocString BODY =
						$"The Deadly danger has events that can {"<i><b>directly kill your duplicants</b></i>".Colored(ColorUtil.RedWarningColor)} or cause extremely deadly things to happen, often with no opportunity to prevent it.  Do not get too attached to your dupes.";

					public static readonly LocString CONTINUE = "Continue".Colored(ColorUtil.RedWarningColor);
				}

				public static class EVENT_CONFIG
				{
					public static readonly LocString TITLE = "Twitch Integration Event Config";

					public static readonly LocString PASTE = "Paste Config Here";
				}

				public static class EVENT_ERROR
				{
					public static readonly LocString TITLE = "Event Error";

					public static readonly LocString BODY_FORMAT =
						"Event {0} crashed while being run: {1}.\nStopping votes.\nPlease report this error (and provide the log) to that event's author.";
				}

				public static class INVALID_CREDENTIALS
				{
					public static readonly LocString TITLE = "Invalid Credentials";

					public static readonly LocString INVALID_FILE =
						"The credentials file was broken and has been reset, please follow the instructions in the README.";

					public static readonly LocString NO_LOGIN =
						"The credentials file does not have a Twitch nickname set. Using an anonymous login instead.";

					public static readonly LocString NICK_CONTAINS_SLASH =
						"The Twitch nickname in the credentials file contained a slash. Using an anonymous login instead. The nickname should be <i>only</i> the name you use to log in to Twitch.";

					public static readonly LocString INVALID_NICK =
						"The Twitch nickname in the credentials file was not a valid nickname. Using an anonymous login instead.";

					public static readonly LocString MALFORMED_OAUTH =
						"Invalid OAuth token!\nThe OAuth token should be composed of only numbers and letters. Using an anonymous login instead.";

					public static readonly LocString EXPIRED_OAUTH =
						"The OAuth token is invalid or has expired. Using an anonymous login instead. Please generate a new token following the instructions in the README.";

					public static readonly LocString UNKNOWN_OAUTH_ERR =
						"An unknown error occured when validating your OAuth token with Twitch. Using an anonymous login instead.";

					public static readonly LocString CONNECTION_OAUTH_ERR =
						"An unknown error occured when validating your OAuth token with Twitch. (No response from server)";
				}

				public static class INVALID_EVENT_CONFIG
				{
					public static readonly LocString TITLE = "Invalid Config";
					public static readonly LocString BODY = "The config provided was invalid";
				}

				public static class NO_VOTES
				{
					public static readonly LocString TITLE = "Cannot Start Vote";
					public static readonly LocString BODY = "No events were able to be drawn.";
				}

				public static class NOT_AUTHENTICATED
				{
					public static readonly LocString TITLE = "Not Authenticated";

					public static readonly LocString BODY =
						"You are not authenticated with Twitch. The vote cannot be started. This may be caused by incorrect or missing credentials.";
				}

				public static class UNKNOWN_SAVE
				{
					public static readonly LocString TITLE = "Unknown Save Version";

					public static readonly LocString BODY =
						"An unknown version of the Twitch Integration config was encountered and the config had to be reset. Your old config has been saved to config_bak.json.";
				}

				public static class UNSAVED_CONFIG
				{
					public static readonly LocString TITLE = "Unsaved Changes";
					public static readonly LocString BODY = "There are unsaved changes.";
					public static readonly LocString SAVE = "Save Changes";
					public static readonly LocString BACK = "Go Back";
					public static readonly LocString DISCARD = "Discard Changes";
				}
			}

			public static class MAIN_MENU
			{
				public static readonly LocString NEW_SETTINGS = "New Twitch Integration Settings";
			}

			public static class PAUSE_MENU
			{
				public static readonly LocString START_VOTES = "Start Voting";
			}

			public static class POCKET_DIMENSION_EXTERIOR_SIDE_SCREEN
			{
				public static readonly LocString NAME = "View Dimension";
				public static readonly LocString TOOLTIP = "View the dimension that this portal is attached to";
			}

			public static class POCKET_DIMENSION_INTERIOR_SIDE_SCREEN
			{
				public static readonly LocString NAME = "View Parent";
				public static readonly LocString TOOLTIP = "View the world that holds this dimension";
			}

			public static class SETTINGS
			{
				public static readonly LocString BUTTON_NAME = "Settings";
			}

			public static class SURPRISE_BOX_SIDE_SCREEN
			{
				public static readonly LocString NAME = "Open";
				public static readonly LocString TOOLTIP = "Open this box to receive your surprise!";
			}
		}

		public static class VOTE_CONTROLLER
		{
			public static readonly LocString START_VOTE_HEADER = "Starting new vote! ";
			public static readonly LocString CHOSEN_VOTE_FORMAT = "The chosen vote was {0} with {1} votes";
			public static readonly LocString NO_VOTES = "No options were voted for";
		}

		public static class VOTE_INFO_FILE
		{
			public static readonly LocString NOT_STARTED = "Voting not yet started";
			public static readonly LocString IN_PROGRESS_FORMAT = "Current Vote: ({0:F0}s)\n{1}";
			public static readonly LocString VOTE_OVER_FORMAT = "Vote Over ({0:F0} seconds to next vote)";
			public static readonly LocString ERROR = "An error occurred";
		}

		public static class WORLDS
		{
			public static class POCKET_DIMENSION
			{
				public static readonly LocString NAME = "Pocket Dimension";
			}
		}
	}
}
