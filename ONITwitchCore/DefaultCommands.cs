using System.Collections.Generic;
using ONITwitchCore.Commands;
using ONITwitchCore.Content;
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
			CommandConfig.Namespaced(
				"SpawnDupe",
				"Spawn Dupe",
				new SpawnDupeCommand(),
				new Dictionary<string, object> { { "MaxDupes", 50.0d } },
				Danger.Small,
				1
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
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
			CommandConfig.Namespaced(
				"AttributeAthleticsUp",
				"Athletics Up",
				new EffectCommand(),
				CustomEffects.AthleticsUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeAthleticsDown",
				"Athletics Down",
				new EffectCommand(),
				CustomEffects.AthleticsDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeConstructionUp",
				"Construction Up",
				new EffectCommand(),
				CustomEffects.ConstructionUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeConstructionDown",
				"Construction Down",
				new EffectCommand(),
				CustomEffects.ConstructionDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeExcavationUp",
				"Excavation Up",
				new EffectCommand(),
				CustomEffects.ExcavationUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeExcavationDown",
				"Excavation Down",
				new EffectCommand(),
				CustomEffects.ExcavationDownEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeStrengthUp",
				"Strength Up",
				new EffectCommand(),
				CustomEffects.StrengthUpEffect.Id,
				Danger.None,
				10
			)
		);
		RegisterCommand(
			CommandConfig.Namespaced(
				"AttributeStrengthDown",
				"Strength Down",
				new EffectCommand(),
				CustomEffects.StrengthDownEffect.Id,
				Danger.None,
				10
			)
		);
	}

	public record struct CommandConfig(
		string Id,
		string Name,
		CommandBase Command,
		object Data,
		Danger Danger,
		int Weight
	)
	{
		public static CommandConfig Namespaced(
			string id,
			string name,
			CommandBase command,
			object data,
			Danger danger,
			int weight
		)
		{
			return new CommandConfig(NamespaceId(id), name, command, data, danger, weight);
		}
	}

	private static string NamespaceId(string id)
	{
		return CommandNamespace + id;
	}

	public static void RegisterCommand(CommandConfig config)
	{
		var eventInst = EventManager.Instance;
		var eventId = eventInst.RegisterEvent(config.Id, config.Name);
		eventInst.AddListenerForEvent(eventId, config.Command.Run);
		DataManager.Instance.AddDataForEvent(eventId, config.Data);
		ConditionsManager.Instance.AddCondition(eventId, config.Command.Condition);
		DangerManager.Instance.SetDanger(eventId, config.Danger);
		TwitchDeckManager.Instance.AddToDeck(DeckUtils.RepeatList(eventId, config.Weight));
	}
}
