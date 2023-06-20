using System.Collections.Generic;
using JetBrains.Annotations;
using ONITwitch.Commands;
using ONITwitch.Content;
using ONITwitch.Content.Entities;
using ONITwitchLib;
using ONITwitchLib.Logger;
using DataManager = ONITwitch.EventLib.DataManager;
using EventGroup = ONITwitch.EventLib.EventGroup;
using EventInfo = ONITwitch.EventLib.EventInfo;

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
				50
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupCommon",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_COMMON,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					SimHashes.Water.ToString(),
					SimHashes.Algae.ToString(),
					SimHashes.OxyRock.ToString(),
					SimHashes.SlimeMold.ToString(),
					SimHashes.Obsidian.ToString(),
					SimHashes.IgneousRock.ToString(),
					SimHashes.SandStone.ToString(),
					SimHashes.Rust.ToString(),
					SimHashes.Sand.ToString(),
					SimHashes.Snow.ToString(),
					SimHashes.Ice.ToString(),
					SimHashes.Carbon.ToString(),
					SimHashes.DirtyWater.ToString(),
					SimHashes.DirtyIce.ToString(),
					SimHashes.Brine.ToString(),
					SimHashes.BrineIce.ToString(),
					SimHashes.Dirt.ToString(),
					SimHashes.MaficRock.ToString(),
					SimHashes.Salt.ToString(),
					SimHashes.Sucrose.ToString(),
					SimHashes.Katairite.ToString(),
				},
				Danger.Small,
				100,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupExotic",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_EXOTIC,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					SimHashes.Diamond.ToString(),
					SimHashes.Ceramic.ToString(),
					SimHashes.Fossil.ToString(),
					SimHashes.Graphite.ToString(),
					SimHashes.EnrichedUranium.ToString(),
					SimHashes.Niobium.ToString(),
					SimHashes.Tungsten.ToString(),
					SimHashes.SuperCoolant.ToString(),
					SimHashes.SuperInsulator.ToString(),
					SimHashes.Resin.ToString(),
				},
				Danger.Small,
				10,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupMetal",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_METAL,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					SimHashes.Cuprite.ToString(),
					SimHashes.FoolsGold.ToString(),
					SimHashes.IronOre.ToString(),
					SimHashes.Electrum.ToString(),
					SimHashes.Cobaltite.ToString(),
					SimHashes.GoldAmalgam.ToString(),
					SimHashes.AluminumOre.ToString(),
				},
				Danger.Small,
				50,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupGas",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_GAS,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					// CO2, polluted O2, and O2 excluded because they are in common
					SimHashes.ChlorineGas.ToString(),
					SimHashes.Hydrogen.ToString(),
					SimHashes.Methane.ToString(),
					SimHashes.SourGas.ToString(),
					SimHashes.Steam.ToString(),
					SimHashes.EthanolGas.ToString(),
				},
				Danger.Small,
				50,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupLiquid",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_LIQUID,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					// Brine, salt water, water, polluted water in common instead
					SimHashes.Chlorine.ToString(),
					SimHashes.CrudeOil.ToString(),
					SimHashes.LiquidCarbonDioxide.ToString(),
					SimHashes.LiquidHydrogen.ToString(),
					SimHashes.LiquidMethane.ToString(),
					SimHashes.LiquidOxygen.ToString(),
					SimHashes.LiquidPhosphorus.ToString(),
					SimHashes.LiquidSulfur.ToString(),
					SimHashes.MoltenGlass.ToString(),
					SimHashes.Naphtha.ToString(),
					SimHashes.Petroleum.ToString(),
					SimHashes.Ethanol.ToString(),
					SimHashes.MoltenSucrose.ToString(),
				},
				Danger.Small,
				50,
				"core.spawn_element"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ElementGroupDeadly",
				STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_DEADLY,
				new SpawnElementPoolCommand(),
				new List<string>
				{
					SimHashes.Magma.ToString(),
					SimHashes.MoltenIron.ToString(),
					SimHashes.MoltenTungsten.ToString(),
					SimHashes.AluminumGas.ToString(),
					SimHashes.RockGas.ToString(),
					SimHashes.SuperCoolantGas.ToString(),
				},
				Danger.Small,
				10,
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
				20,
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
				20,
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
				20,
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
				20,
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
				20,
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
				20,
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
				20,
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
				20,
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
				10
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SnowBedrooms",
				STRINGS.ONITWITCH.EVENTS.BEDROOM_SNOW,
				new FillBedroomCommand(),
				SimHashes.Snow.ToString(),
				Danger.Small,
				20,
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
				20,
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
				30,
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
				30,
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
				10,
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
				10,
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
				2,
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
				2,
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
				2,
				"core.flood"
			)
		);
		RegisterCommand(
			new CommandInfo("IceAge", STRINGS.ONITWITCH.EVENTS.ICE_AGE, new IceAgeCommand(), null, Danger.Extreme, 1)
		);
		RegisterCommand(
			new CommandInfo(
				"GlobalWarming",
				STRINGS.ONITWITCH.EVENTS.GLOBAL_WARMING,
				new GlobalWarmingCommand(),
				null,
				Danger.High,
				1
			)
		);
		RegisterCommand(new CommandInfo("Pee", STRINGS.ONITWITCH.EVENTS.PEE, new PeeCommand(), null, Danger.Small, 30));
		RegisterCommand(
			new CommandInfo("Kill", STRINGS.ONITWITCH.EVENTS.KILL_DUPE, new KillDupeCommand(), null, Danger.Deadly, 1)
		);
		RegisterCommand(
			new CommandInfo(
				"TileTempDown",
				STRINGS.ONITWITCH.EVENTS.TILE_TEMP_DOWN,
				new TileTempCommand(),
				-40.0d,
				Danger.Medium,
				10,
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
				10,
				"core.tile_temp"
			)
		);
		RegisterCommand(
			new CommandInfo("PartyTime", STRINGS.ONITWITCH.EVENTS.PARTY_TIME, new PartyTimeCommand(), 300.0d, Danger.None, 30)
		);
		RegisterCommand(
			new CommandInfo("PoisonDupes", STRINGS.ONITWITCH.EVENTS.POISON, new PoisonDupesCommand(), null, Danger.High, 20)
		);
		RegisterCommand(
			new CommandInfo(
				"Poopsplosion",
				STRINGS.ONITWITCH.EVENTS.POOPSPLOSION,
				new PoopsplosionCommand(),
				null,
				Danger.Small,
				50
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabGold",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_GOLD,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Gold.ToString() }, { "Count", 50.0d } },
				Danger.None,
				30,
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
				30,
				"core.rain_prefab"
			)
		);
		RegisterCommand(
			new CommandInfo(
				"RainPrefabDiamond",
				STRINGS.ONITWITCH.EVENTS.RAIN_PREFAB_DIAMOND,
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Diamond.ToString() }, { "Count", 25.0d } },
				Danger.None,
				10,
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
				20,
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
				20,
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
				10,
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
				10
			)
		);
		RegisterCommand(
			new CommandInfo("SkillPoints", STRINGS.ONITWITCH.EVENTS.SKILL_POINTS, new SkillCommand(), 0.33d, Danger.None, 20)
		);
		RegisterCommand(
			new CommandInfo(
				"SleepyDupes",
				STRINGS.ONITWITCH.EVENTS.SLEEPY_DUPES,
				new SleepyDupesCommand(),
				null,
				Danger.Medium,
				20
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SnazzySuit",
				STRINGS.ONITWITCH.EVENTS.SPAWN_SNAZZY_SUIT,
				new SnazzySuitCommand(),
				null,
				Danger.None,
				20
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnGlitterPuft",
				STRINGS.ONITWITCH.EVENTS.SPAWN_GLITTER_PUFT,
				new SpawnPrefabCommand(),
				GlitterPuftConfig.Id,
				Danger.None,
				20
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnVacillatorCharge",
				STRINGS.ONITWITCH.EVENTS.SPAWN_VACILLATOR_CHARGE,
				new SpawnPrefabCommand(),
				GeneShufflerRechargeConfig.ID,
				Danger.None,
				5
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnAtmoSuit",
				STRINGS.ONITWITCH.EVENTS.SPAWN_ATMO_SUIT,
				new SpawnPrefabCommand(),
				AtmoSuitConfig.ID,
				Danger.None,
				5
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnCrab",
				STRINGS.ONITWITCH.EVENTS.SPAWN_CRAB,
				new SpawnPrefabCommand(),
				CrabConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			new CommandInfo(
				"SpawnMooComet",
				STRINGS.ONITWITCH.EVENTS.SPAWN_MOO_COMET,
				new SpawnPrefabCommand(),
				GassyMooCometConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			new CommandInfo(
				"StressAdd",
				STRINGS.ONITWITCH.EVENTS.STRESS_ADD,
				new StressCommand(),
				+0.75d,
				Danger.Medium,
				10,
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
				20,
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
				5
			)
		);
		RegisterCommand(
			new CommandInfo(
				"ResearchTech",
				STRINGS.ONITWITCH.EVENTS.RESEARCH_TECH,
				new ResearchTechCommand(),
				null,
				Danger.None,
				10
			)
		);

		RegisterCommand(
			new CommandInfo(
				"Eclipse",
				STRINGS.ONITWITCH.EVENTS.ECLIPSE,
				new EclipseCommand(),
				(double) (3 * Constants.SECONDS_PER_CYCLE),
				Danger.Small,
				10
			)
		);

		RegisterCommand(
			new CommandInfo(
				"PocketDimension",
				STRINGS.ONITWITCH.EVENTS.POCKET_DIMENSION,
				new PocketDimensionCommand(),
				null,
				Danger.None,
				10
			)
		);

		RegisterCommand(
			new CommandInfo(
				"SurpriseBox",
				STRINGS.ONITWITCH.EVENTS.SURPRISE_BOX,
				new SurpriseBoxCommand(),
				null,
				Danger.None,
				20,
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
				10
			)
		);

		RegisterCommand(
			new CommandInfo(
				"MorphCritters",
				STRINGS.ONITWITCH.EVENTS.MORPH_CRITTERS,
				new MorphCommand(),
				null,
				Danger.Small,
				5
			)
		);

		RegisterCommand(
			new CommandInfo(
				"Fart",
				STRINGS.ONITWITCH.EVENTS.FART,
				new FartCommand(),
				25.0d,
				Danger.Small,
				30
			)
		);

		RegisterCommand(
			new CommandInfo(
				"SpiceFood",
				STRINGS.ONITWITCH.EVENTS.SPICE_FOOD,
				new SpiceFoodCommand(),
				null,
				Danger.None,
				30
			)
		);
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
}
