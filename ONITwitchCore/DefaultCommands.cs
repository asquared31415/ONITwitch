using System.Collections.Generic;
using ONITwitchCore.Commands;
using ONITwitchCore.Config;
using ONITwitchCore.Content;
using ONITwitchCore.Content.Entities;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventManager = EventLib.EventManager;

namespace ONITwitchCore;

public static class DefaultCommands
{
	public const string CommandNamespace = "ONITwitch.";

	public static void SetupCommands()
	{
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnDupe",
				"Spawn Dupe",
				new SpawnDupeCommand(),
				new Dictionary<string, object> { { "MaxDupes", 50.0d } },
				Danger.Small,
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupCommon",
				"Spawn Common Element",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupExotic",
				"Spawn Exotic Element",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupMetal",
				"Spawn Metal",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupGas",
				"Spawn Gas",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupLiquid",
				"Spawn Liquid",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ElementGroupDeadly",
				"Spawn Deadly Element",
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
				1
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeAthleticsUp",
				"Athletics Up",
				new EffectCommand(),
				CustomEffects.AthleticsUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeAthleticsDown",
				"Athletics Down",
				new EffectCommand(),
				CustomEffects.AthleticsDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeConstructionUp",
				"Construction Up",
				new EffectCommand(),
				CustomEffects.ConstructionUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeConstructionDown",
				"Construction Down",
				new EffectCommand(),
				CustomEffects.ConstructionDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeExcavationUp",
				"Excavation Up",
				new EffectCommand(),
				CustomEffects.ExcavationUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeExcavationDown",
				"Excavation Down",
				new EffectCommand(),
				CustomEffects.ExcavationDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeStrengthUp",
				"Strength Up",
				new EffectCommand(),
				CustomEffects.StrengthUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"AttributeStrengthDown",
				"Strength Down",
				new EffectCommand(),
				CustomEffects.StrengthDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced("BansheeWail", "Banshee Wail", new BansheeWailCommand(), null, Danger.Small, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SnowBedrooms",
				"Snowy Bedrooms",
				new FillBedroomCommand(),
				SimHashes.Snow.ToString(),
				Danger.Small,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SlimeBedrooms",
				"Slimy Bedrooms",
				new FillBedroomCommand(),
				SimHashes.SlimeMold.ToString(),
				Danger.Small,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodWater",
				"Water Flood",
				new ElementFloodCommand(),
				SimHashes.Water.ToString(),
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodPollutedWater",
				"Polluted Water Flood",
				new ElementFloodCommand(),
				SimHashes.DirtyWater.ToString(),
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodEthanol",
				"Ethanol Flood",
				new ElementFloodCommand(),
				SimHashes.Ethanol.ToString(),
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodOil",
				"Oil Flood",
				new ElementFloodCommand(),
				SimHashes.CrudeOil.ToString(),
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodLava",
				"Lava Flood",
				new ElementFloodCommand(),
				SimHashes.Magma.ToString(),
				Danger.Deadly,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodGold",
				"Gold Rain",
				new ElementFloodCommand(),
				SimHashes.Magma.ToString(),
				Danger.Deadly,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"FloodNuclearWaste",
				"Nuclear Waste Disposal Accident",
				new ElementFloodCommand(),
				SimHashes.NuclearWaste.ToString(),
				Danger.High,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced("IceAge", "Ice Age", new IceAgeCommand(), null, Danger.Extreme, 10)
		);
		RegisterCommand(CommandInfo.Namespaced("Pee", "Gold Rain", new PeeCommand(), null, Danger.Small, 10));
		RegisterCommand(
			CommandInfo.Namespaced("Kill", "Kill Duplicant", new KillDupeCommand(), null, Danger.Deadly, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced("TileTempDown", "Icy Floors", new TileTempCommand(), -40.0d, Danger.Medium, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced("TileTempUp", "Floor is Lava", new TileTempCommand(), +40.0d, Danger.Medium, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced("PartyTime", "Party Time", new PartyTimeCommand(), 300.0d, Danger.None, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced("PoisonDupes", "Poison Dupes", new PoisonDupesCommand(), null, Danger.High, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced("Poopsplosion", "Poopsplosion", new PoopsplosionCommand(), null, Danger.Small, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabGold",
				"Gold Rain",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Gold.ToString() }, { "Count", 50.0d } },
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabMorb",
				"Morbin' Time",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", GlomConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabDiamond",
				"Diamond Rain",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", SimHashes.Diamond.ToString() }, { "Count", 25.0d } },
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabSlickster",
				"Slickster Rain",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", OilFloaterConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabPacu",
				"Pacu Rain",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", PacuConfig.ID }, { "Count", 10.0d } },
				Danger.Small,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"RainPrefabBee",
				"Bee Infestation",
				new RainPrefabCommand(),
				new Dictionary<string, object> { { "PrefabId", BeeConfig.ID }, { "Count", 10.0d } },
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ReduceOxygen",
				"Un-include Oxygen",
				new ReduceOxygenCommand(),
				0.20d,
				Danger.High,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced("SkillPoints", "Skilling Spree", new SkillCommand(), 0.33d, Danger.None, 10)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SleepyDupes",
				"Sleepy Dupes",
				new SleepyDupesCommand(),
				null,
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SnazzySuit",
				"Spawn Snazzy Suit",
				new SnazzySuitCommand(),
				null,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnGlitterPuft",
				"Spawn Glitter Puft",
				new SpawnPrefabCommand(),
				GlitterPuftConfig.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnVacillatorCharge",
				"Spawn Vacillator Charge",
				new SpawnPrefabCommand(),
				GeneShufflerRechargeConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnAtmoSuit",
				"Spawn Atmo Suit",
				new SpawnPrefabCommand(),
				AtmoSuitConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnCrab",
				"Spawn Pokeshell",
				new SpawnPrefabCommand(),
				CrabConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"SpawnMooComet",
				"Spawn Gassy Moo Comet",
				new SpawnPrefabCommand(),
				GassyMooCometConfig.ID,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"StressAdd",
				"Add Stress",
				new StressCommand(),
				+0.75d,
				Danger.Medium,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"StressRemove",
				"Remove Stress",
				new StressCommand(),
				-0.75d,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"Surprise",
				"Surprise!!!",
				new SurpriseCommand(),
				null,
				// will only choose a command that is within the expected danger range
				// TODO: maybe handle this specially and have a "all dangers" option that makes danger ignored?
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"Uninsulate",
				"Uninsulate Tiles",
				new UninsulateCommand(),
				null,
				Danger.Extreme,
				10
			)
		);
		RegisterCommand(
			CommandInfo.Namespaced(
				"ResearchTech",
				"Research Technology",
				new ResearchTechCommand(),
				null,
				Danger.None,
				10
			)
		);

		// update user configs
		UserCommandConfigManager.Instance.Reload();
	}

	public record struct CommandInfo(
		string Id,
		string Name,
		CommandBase Command,
		object Data,
		Danger Danger,
		int Weight
	)
	{
		public static CommandInfo Namespaced(
			string id,
			string name,
			CommandBase command,
			object data,
			Danger danger,
			int weight
		)
		{
			return new CommandInfo(NamespaceId(id), name, command, data, danger, weight);
		}
	}

	public static string NamespaceId(string id)
	{
		return CommandNamespace + id;
	}

	public static void RegisterCommand(CommandInfo info)
	{
		var eventInst = EventManager.Instance;
		var eventId = eventInst.RegisterEvent(info.Id, info.Name);
		eventInst.AddListenerForEvent(eventId, info.Command.Run);
		DataManager.Instance.SetDataForEvent(eventId, info.Data);
		ConditionsManager.Instance.AddCondition(eventId, info.Command.Condition);
		DangerManager.Instance.SetDanger(eventId, info.Danger);
		TwitchDeckManager.Instance.AddToDeck(eventId, info.Weight);
	}

	public static void ReloadData(Dictionary<string, CommandConfig> userConfig)
	{
		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;
		foreach (var (id, config) in userConfig)
		{
			var eventId = eventInst.GetEventByID(id);
			if (eventId != null)
			{
				eventInst.RenameEvent(eventId, config.FriendlyName);
				dataInst.SetDataForEvent(eventId, config.Data);
				deckInst.AddToDeck(eventId, config.Weight);
			}
		}
	}
}
