using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitch.Commands;
using ONITwitch.Content;
using ONITwitch.Content.EntityConfigs;
using ONITwitchLib;
using ONITwitchLib.Logger;
using DataManager = ONITwitch.EventLib.DataManager;
using EventGroup = ONITwitch.EventLib.EventGroup;
using EventInfo = ONITwitch.EventLib.EventInfo;
using static ONITwitchLib.Consts;

namespace ONITwitch;

internal static class DefaultCommands
{
	public static void SetupCommands()
	{
		Log.Info("Setting up default commands");
		RegisterCommand(
			new CommandInfo(
				"SpawnDupe",
				STRINGS.ONITWITCH.EVENTS.SPAWN_DUPE,
				new SpawnDupeCommand(),
				new Dictionary<string, object> { { "MaxDupes", 30.0d } },
				Danger.Small,
				EventWeight.Frequent
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupCommon",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_COMMON,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					{ SimHashes.Algae.ToString(), 1400 },
					{ SimHashes.OxyRock.ToString(), 1400 },
					{ SimHashes.SlimeMold.ToString(), 1400 },
					{ SimHashes.IgneousRock.ToString(), 1400 },
					{ SimHashes.Rust.ToString(), 1400 },
					{ SimHashes.Sand.ToString(), 1400 },
					{ SimHashes.Ice.ToString(), 1400 },
					{ SimHashes.Carbon.ToString(), 1400 },
					{ SimHashes.Dirt.ToString(), 1400 },
					{ SimHashes.Salt.ToString(), 1400 },
					{ SimHashes.Sucrose.ToString(), 1400 },
				},
				Danger.Small,
				EventWeight.Frequent,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupExotic",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_EXOTIC,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					{ SimHashes.Diamond.ToString(), 1000 },
					{ SimHashes.Ceramic.ToString(), 2000 },
					{ SimHashes.Fossil.ToString(), 1400 },
					{ SimHashes.Graphite.ToString(), 1000 },
					{ SimHashes.EnrichedUranium.ToString(), 2000 },
					{ SimHashes.Niobium.ToString(), 500 },
					{ SimHashes.Tungsten.ToString(), 1200 },
					// (20/2)*5 = 50 = 5 tiles in a cooling loop
					{ SimHashes.SuperCoolant.ToString(), 20 },
					// 5 pipes worth
					{ SimHashes.SuperInsulator.ToString(), 800 },
					{ SimHashes.Resin.ToString(), 500 },
				},
				Danger.Small,
				EventWeight.Uncommon,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupMetal",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_METAL,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					{ SimHashes.Cuprite.ToString(), 1500 },
					{ SimHashes.FoolsGold.ToString(), 1500 },
					{ SimHashes.IronOre.ToString(), 1500 },
					{ SimHashes.Electrum.ToString(), 1500 },
					{ SimHashes.Cobaltite.ToString(), 1500 },
					{ SimHashes.GoldAmalgam.ToString(), 1500 },
					{ SimHashes.AluminumOre.ToString(), 1500 },
				},
				Danger.Small,
				EventWeight.Frequent,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupGas",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_GAS,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					// CO2, polluted O2, and O2 excluded, they're too boring
					{ SimHashes.ChlorineGas.ToString(), 10 },
					{ SimHashes.Hydrogen.ToString(), 10 },
					{ SimHashes.Methane.ToString(), 20 },
					{ SimHashes.SourGas.ToString(), 10 },
					{ SimHashes.Steam.ToString(), 10 },
					{ SimHashes.EthanolGas.ToString(), 10 },
				},
				Danger.Small,
				EventWeight.Frequent,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupDeadly",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_DEADLY,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					// All of these are one tile surrounded by insulation
					{ SimHashes.Magma.ToString(), 4000 },
					{ SimHashes.MoltenIron.ToString(), 4000 },
					// really bad SHC
					{ SimHashes.MoltenTungsten.ToString(), 5000 },
					// Aluminium and Rock Gas have decent SHC (~1), really hot
					{ SimHashes.AluminumGas.ToString(), 2000 },
					{ SimHashes.RockGas.ToString(), 2000 },

					// huge SHC, but only 850K. Condenses at 710K. There's a mechanism to make sure
					// that nothing spawns less than 300 degrees below its melting point if it's above 200C though.
					// So the final temp will be 1010K and really high SHC.
					{ SimHashes.SuperCoolantGas.ToString(), 1000 },
				},
				Danger.Small,
				EventWeight.Uncommon,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupLiquid",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_LIQUID,
				new SpawnElementPoolCommand(),
				new Dictionary<string, float>
				{
					// several waters are in common instead, they're boring
					// 100 tiles of gas once these heat up
					{ SimHashes.Chlorine.ToString(), 20 },
					{ SimHashes.LiquidHydrogen.ToString(), 20 },
					{ SimHashes.LiquidOxygen.ToString(), 20 },


					// only one tile spawns of these
					{ SimHashes.LiquidPhosphorus.ToString(), 1500 },
					{ SimHashes.MoltenGlass.ToString(), 2500 },
					{ SimHashes.MoltenSucrose.ToString(), 1500 },

					// Liquid sulfur is just barely under 200 degrees, but it doesn't have a ton of heat
					{ SimHashes.LiquidSulfur.ToString(), 500 },

					{ SimHashes.Naphtha.ToString(), 500 },
					{ SimHashes.Petroleum.ToString(), 500 },
					{ SimHashes.Ethanol.ToString(), 500 },
					{ SimHashes.CrudeOil.ToString(), 500 },
				},
				Danger.Small,
				EventWeight.Frequent,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeAthleticsUp",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_ATHLETICS_UP,
				new EffectCommand(),
				CustomEffects.AthleticsUpEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeAthleticsDown",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_ATHLETICS_DOWN,
				new EffectCommand(),
				CustomEffects.AthleticsDownEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeConstructionUp",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_CONSTRUCTION_UP,
				new EffectCommand(),
				CustomEffects.ConstructionUpEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeConstructionDown",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_CONSTRUCTION_DOWN,
				new EffectCommand(),
				CustomEffects.ConstructionDownEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeExcavationUp",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_EXCAVATION_UP,
				new EffectCommand(),
				CustomEffects.ExcavationUpEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeExcavationDown",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_EXCAVATION_DOWN,
				new EffectCommand(),
				CustomEffects.ExcavationDownEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeStrengthUp",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_STRENGTH_UP,
				new EffectCommand(),
				CustomEffects.StrengthUpEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"AttributeStrengthDown",
				STRINGS.ONITWITCH.EVENTS.ATTRIBUTE_STRENGTH_DOWN,
				new EffectCommand(),
				CustomEffects.StrengthDownEffect.Id,
				Danger.None,
				EventWeight.Uncommon,
				"core.attribute_mod"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"BansheeWail",
				STRINGS.ONITWITCH.EVENTS.BANSHEE_WAIL,
				new BansheeWailCommand(),
				null,
				Danger.Small,
				EventWeight.Uncommon
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SnowBedrooms",
				STRINGS.ONITWITCH.EVENTS.BEDROOM_SNOW,
				new FillBedroomCommand(),
				SimHashes.Snow.ToString(),
				Danger.Small,
				EventWeight.Common,
				"core.bedroom_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SlimeBedrooms",
				STRINGS.ONITWITCH.EVENTS.BEDROOMS_SLIME,
				new FillBedroomCommand(),
				SimHashes.SlimeMold.ToString(),
				Danger.Small,
				EventWeight.Common,
				"core.bedroom_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodWater",
				STRINGS.ONITWITCH.EVENTS.FLOOD_WATER,
				new ElementFloodCommand(),
				SimHashes.Water.ToString(),
				Danger.Medium,
				EventWeight.Common,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodPollutedWater",
				STRINGS.ONITWITCH.EVENTS.FLOOD_POLLUTED_WATER,
				new ElementFloodCommand(),
				SimHashes.DirtyWater.ToString(),
				Danger.Medium,
				EventWeight.Common,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodEthanol",
				STRINGS.ONITWITCH.EVENTS.FLOOD_ETHANOL,
				new ElementFloodCommand(),
				SimHashes.Ethanol.ToString(),
				Danger.Medium,
				EventWeight.Uncommon,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodOil",
				STRINGS.ONITWITCH.EVENTS.FLOOD_OIL,
				new ElementFloodCommand(),
				SimHashes.CrudeOil.ToString(),
				Danger.Medium,
				EventWeight.Uncommon,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodLava",
				STRINGS.ONITWITCH.EVENTS.FLOOD_LAVA,
				new ElementFloodCommand(),
				SimHashes.Magma.ToString(),
				Danger.Deadly,
				EventWeight.VeryRare,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodGold",
				STRINGS.ONITWITCH.EVENTS.FLOOD_GOLD,
				new ElementFloodCommand(),
				SimHashes.MoltenGold.ToString(),
				Danger.Deadly,
				EventWeight.VeryRare,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"FloodNuclearWaste",
				STRINGS.ONITWITCH.EVENTS.FLOOD_NUCLEAR_WASTE,
				new ElementFloodCommand(),
				SimHashes.NuclearWaste.ToString(),
				Danger.High,
				EventWeight.VeryRare,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"IceAge",
				STRINGS.ONITWITCH.EVENTS.ICE_AGE,
				new IceAgeCommand(),
				null,
				Danger.Extreme,
				EventWeight.AlmostNever
			)
		);
		RegisterCommand(
			new CommandInfo(
				"GlobalWarming",
				STRINGS.ONITWITCH.EVENTS.GLOBAL_WARMING,
				new GlobalWarmingCommand(),
				null,
				Danger.High,
				EventWeight.AlmostNever
			)
		);
		RegisterCommand(
			new CommandInfo(
				"Pee",
				STRINGS.ONITWITCH.EVENTS.PEE,
				new PeeCommand(),
				null,
				Danger.Small,
				EventWeight.Frequent
			)
		);
		RegisterCommand(
			new CommandInfo(
				"Kill",
				STRINGS.ONITWITCH.EVENTS.KILL_DUPE,
				new KillDupeCommand(),
				null,
				Danger.Deadly,
				EventWeight.AlmostNever
			)
		);
		RegisterCommand(
			new CommandInfo(
				"TileTempDown",
				STRINGS.ONITWITCH.EVENTS.TILE_TEMP_DOWN,
				new TileTempCommand(),
				-40.0d,
				Danger.Medium,
				EventWeight.Uncommon,
				"core.tile_temp"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"TileTempUp",
				STRINGS.ONITWITCH.EVENTS.TILE_TEMP_UP,
				new TileTempCommand(),
				+40.0d,
				Danger.Medium,
				EventWeight.Uncommon,
				"core.tile_temp"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"PartyTime",
				STRINGS.ONITWITCH.EVENTS.PARTY_TIME,
				new PartyTimeCommand(),
				300.0d,
				Danger.None,
				EventWeight.Common
			)
		);
		RegisterCommand(
			new CommandInfo(
				"PoisonDupes",
				STRINGS.ONITWITCH.EVENTS.POISON,
				new PoisonDupesCommand(),
				null,
				Danger.High,
				EventWeight.Uncommon
			)
		);
		RegisterCommand(
			new CommandInfo(
				"Poopsplosion",
				STRINGS.ONITWITCH.EVENTS.POOPSPLOSION,
				new PoopsplosionCommand(),
				null,
				Danger.Small,
				EventWeight.Frequent
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabGold",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_GOLD,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Gold.ToString() }, { "Count", 50.0d } },
				Danger.None,
				EventWeight.Common,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabMorb",
				OniTwitchMod.ModIntegration.IsModPresentAndActive("AmogusMorb")
					? STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_MORB_AMORBUS
					: STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_MORB_NORMAL,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", GlomConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				EventWeight.Common,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabDiamond",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_DIAMOND,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Diamond.ToString() }, { "Count", 100.0d } },
				Danger.None,
				EventWeight.Rare,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabSlickster",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_SLICKSTER,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", OilFloaterConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				EventWeight.Uncommon,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabPacu",
				OniTwitchMod.ModIntegration.IsModPresentAndActive("WeebPacu")
					? STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_PACU_PACUWU
					: STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_PACU_NORMAL,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", PacuConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				EventWeight.Uncommon,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabBee",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_BEE,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", BeeConfig.ID }, { "Count", 10.0d } },
				Danger.Medium,
				EventWeight.Uncommon,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ReduceOxygen",
				STRINGS.ONITWITCH.EVENTS.REDUCE_OXYGEN,
				new ReduceOxygenCommand(),
				0.20d,
				Danger.High,
				EventWeight.Uncommon
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SkillPoints",
				STRINGS.ONITWITCH.EVENTS.SKILL_POINTS,
				new SkillCommand(),
				0.33d,
				Danger.None,
				EventWeight.Common
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SleepyDupes",
				STRINGS.ONITWITCH.EVENTS.SLEEPY_DUPES,
				new SleepyDupesCommand(),
				null,
				Danger.Medium,
				EventWeight.Common
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SnazzySuit",
				STRINGS.ONITWITCH.EVENTS.SPAWN_SNAZZY_SUIT,
				new SnazzySuitCommand(),
				null,
				Danger.None,
				EventWeight.Uncommon
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnGlitterPuft",
				STRINGS.ONITWITCH.EVENTS.SPAWN_GLITTER_PUFT,
				new SpawnPrefabCommand(),
				GlitterPuftConfig.Id,
				Danger.None,
				EventWeight.Common
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnVacillatorCharge",
				STRINGS.ONITWITCH.EVENTS.SPAWN_VACILLATOR_CHARGE,
				new SpawnPrefabCommand(),
				GeneShufflerRechargeConfig.ID,
				Danger.None,
				EventWeight.Rare
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnAtmoSuit",
				STRINGS.ONITWITCH.EVENTS.SPAWN_ATMO_SUIT,
				new SpawnPrefabCommand(),
				AtmoSuitConfig.ID,
				Danger.None,
				EventWeight.Rare
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnCrab",
				STRINGS.ONITWITCH.EVENTS.SPAWN_CRAB,
				new SpawnPrefabCommand(),
				CrabConfig.ID,
				Danger.None,
				EventWeight.Uncommon
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnMooComet",
				STRINGS.ONITWITCH.EVENTS.SPAWN_MOO_COMET,
				new SpawnPrefabCommand(),
				GassyMooCometConfig.ID,
				Danger.None,
				EventWeight.Rare
			)
		);
		RegisterCommand(
			new CommandInfo(
				"StressAdd",
				STRINGS.ONITWITCH.EVENTS.STRESS_ADD,
				new StressCommand(),
				+0.75d,
				Danger.Medium,
				EventWeight.Common,
				"core.stress"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"StressRemove",
				STRINGS.ONITWITCH.EVENTS.STRESS_REMOVE,
				new StressCommand(),
				-0.75d,
				Danger.None,
				EventWeight.Common,
				"core.stress"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"Surprise",
				STRINGS.ONITWITCH.EVENTS.SURPRISE,
				new SurpriseCommand(),
				null,
				// will only choose a command that is within the expected danger range
				// but this command should ignore danger settings
				null,
				// This is an exception to the normal weights, it's twice as frequent as "frequent" because it's special.
				80,
				"core.surprise"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"Uninsulate",
				STRINGS.ONITWITCH.EVENTS.UNINSULATE,
				new UninsulateCommand(),
				null,
				Danger.Extreme,
				EventWeight.Rare
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ResearchTech",
				STRINGS.ONITWITCH.EVENTS.RESEARCH_TECH,
				new ResearchTechCommand(),
				null,
				Danger.None,
				EventWeight.Common
			)
		);

		RegisterCommand(
			new CommandInfo(
				"Eclipse",
				STRINGS.ONITWITCH.EVENTS.ECLIPSE,
				new EclipseCommand(),
				(double) (3 * Constants.SECONDS_PER_CYCLE),
				Danger.Small,
				EventWeight.Uncommon
			)
		);

		RegisterCommand(
			new CommandInfo(
				"PocketDimension",
				STRINGS.ONITWITCH.EVENTS.POCKET_DIMENSION,
				new PocketDimensionCommand(),
				null,
				Danger.None,
				EventWeight.Rare
			)
		);

		RegisterCommand(
			new CommandInfo(
				"SurpriseBox",
				STRINGS.ONITWITCH.EVENTS.SURPRISE_BOX,
				new SurpriseBoxCommand(),
				null,
				Danger.None,
				EventWeight.Common,
				"core.surprise"
			)
		);

		RegisterCommand(
			new CommandInfo(
				"GeyserModification",
				STRINGS.ONITWITCH.EVENTS.GEYSER_MODIFICATION,
				new GeyserModificationCommand(),
				null,
				Danger.Medium,
				EventWeight.Uncommon
			)
		);

		RegisterCommand(
			new CommandInfo(
				"MorphCritters",
				STRINGS.ONITWITCH.EVENTS.MORPH_CRITTERS,
				new MorphCommand(),
				null,
				Danger.Small,
				EventWeight.Rare
			)
		);

		RegisterCommand(
			new CommandInfo(
				"Fart",
				STRINGS.ONITWITCH.EVENTS.FART,
				new FartCommand(),
				25.0d,
				Danger.Small,
				EventWeight.Common
			)
		);

		RegisterCommand(
			new CommandInfo(
				"SpiceFood",
				STRINGS.ONITWITCH.EVENTS.SPICE_FOOD,
				new SpiceFoodCommand(),
				null,
				Danger.None,
				EventWeight.Uncommon
			)
		);
	}

	private static void RegisterCommand(CommandInfo info)
	{
		var deckInst = TwitchDeckManager.Instance;
		EventInfo eventInfo;
		if (info.Group != null)
		{
			var group = deckInst.GetGroup(info.Group);
			if (group == null)
			{
				group = EventGroup.GetOrCreateGroup(info.Group);
				deckInst.AddGroup(group);
			}

			eventInfo = group.AddEvent(info.Id, info.Weight, info.Name);
		}
		else
		{
			(eventInfo, var group) = EventGroup.DefaultSingleEventGroup(info.Id, info.Weight, info.Name);
			deckInst.AddGroup(group);
		}

		eventInfo.AddListener(info.Command.Run);
		DataManager.Instance.SetDataForEvent(eventInfo, info.Data);
		eventInfo.AddCondition(info.Command.Condition);
		eventInfo.Danger = info.Danger;
	}

	private record struct CommandInfo(
		[NotNull] string Id,
		[NotNull] string Name,
		[NotNull] CommandBase Command,
		[CanBeNull] object Data,
		[CanBeNull] Danger? Danger,
		int Weight,
		[CanBeNull] string Group = null
	);
}
