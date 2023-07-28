using System;
using System.Collections.Generic;
using System.Linq;
using Delaunay.Geo;
using HarmonyLib;
using JetBrains.Annotations;
using Klei;
using ONITwitch.Cmps.PocketDimension;
using ONITwitch.Content.Buildings;
using ONITwitch.Content.Entities;
using ONITwitchLib.Utils;
using ProcGen;
using TUNING;
using UnityEngine;

namespace ONITwitch.Content;

/// <summary>
///     Provides methods for adding new pocket dimensions to the generation pool and to generate pocket dimensions.
/// </summary>
[PublicAPI]
public static class PocketDimensionGenerator
{
	// WARNING: this must have at least one entry with no required skill ID, to avoid crashes 
	private static readonly List<BasePocketDimensionGeneration> PocketDimensionSettings = new()
	{
		// ========================================
		// Templates
		// ========================================

		// Trollface indestructible border
		new TemplatePocketDimensionGeneration(
			1f,
			SubWorld.ZoneType.Space,
			"TwitchIntegration/TrollFace"
		),
		// Meep face
		new TemplatePocketDimensionGeneration(
			1f,
			SubWorld.ZoneType.CrystalCaverns,
			PocketDimension.MeepTemplate
		),
		// Aquarium
		new TemplatePocketDimensionGeneration(
			5f,
			SubWorld.ZoneType.Ocean,
			"TwitchIntegration/PacuTank"
		),
		// Sus
		new TemplatePocketDimensionGeneration(
			2f,
			SubWorld.ZoneType.Ocean,
			"TwitchIntegration/Sus",
			"Mining3"
		),

		// ========================================
		// World Themes
		// ========================================

		// Sandstone, algae, dirt
		new NoisePocketDimensionGeneration(
			3f,
			SubWorld.ZoneType.Sandstone,
			new List<SimHashes>
			{
				SimHashes.SandStone,
				SimHashes.SandStone,
				SimHashes.SandStone,
				SimHashes.Algae,
				SimHashes.Fertilizer,
				SimHashes.Dirt,
			},
			0.05f,
			0.15f
		),
		// icy biome
		new NoisePocketDimensionGeneration(
			3f,
			SubWorld.ZoneType.FrozenWastes,
			new List<SimHashes>
			{
				SimHashes.Vacuum,
				SimHashes.Snow,
				SimHashes.Ice,
				SimHashes.Ice,
				SimHashes.DirtyIce,
				SimHashes.BrineIce,
				SimHashes.Wolframite,
			},
			0.2f,
			0.2f
		),
		// sulfur biome
		new NoisePocketDimensionGeneration(
			3f,
			SubWorld.ZoneType.Wasteland,
			new List<SimHashes>
			{
				SimHashes.Vacuum,
				SimHashes.Sand,
				SimHashes.IgneousRock,
				SimHashes.Sand,
				SimHashes.IgneousRock,
				SimHashes.IgneousRock,
				SimHashes.Sulfur,
			},
			// stretched out tall
			0.05f,
			0.15f
		),
		// slime biome
		new NoisePocketDimensionGeneration(
			3f,
			SubWorld.ZoneType.BoggyMarsh,
			new List<SimHashes>
			{
				SimHashes.ContaminatedOxygen,
				SimHashes.DirtyWater,
				SimHashes.SlimeMold,
				SimHashes.Algae,
				SimHashes.SlimeMold,
				SimHashes.SedimentaryRock,
				SimHashes.Clay,
				SimHashes.GoldAmalgam,
			},
			0.1f,
			0.1f
		),
		// ocean biome
		new NoisePocketDimensionGeneration(
			4f,
			SubWorld.ZoneType.Ocean,
			new List<SimHashes>
			{
				SimHashes.Vacuum,
				SimHashes.Hydrogen,
				SimHashes.Vacuum,
				SimHashes.SaltWater,
				SimHashes.SaltWater,
				SimHashes.Sand,
				SimHashes.Sand,
				SimHashes.Salt,
				SimHashes.BleachStone,
				SimHashes.Granite,
				SimHashes.SedimentaryRock,
				SimHashes.SedimentaryRock,
				SimHashes.Granite,
				SimHashes.Fossil,
			},
			// very wide
			0.04f,
			0.2f,
			"Mining1",
			new List<string>
			{
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
				CrabConfig.ID,
			}
		),
		// metallic biome
		new NoisePocketDimensionGeneration(
			4f,
			SubWorld.ZoneType.Metallic,
			new List<SimHashes>
			{
				SimHashes.Vacuum,
				SimHashes.Vacuum,
				SimHashes.IgneousRock,
				SimHashes.Vacuum,
				SimHashes.Carbon,
				SimHashes.Vacuum,
				SimHashes.AluminumOre,
				SimHashes.Vacuum,
				SimHashes.Cobaltite,
				SimHashes.Vacuum,
				SimHashes.GoldAmalgam,
				SimHashes.Vacuum,
				SimHashes.IronOre,
			},
			// smaller blobs
			0.3f,
			0.3f
		),
		// radioactive biome
		new NoisePocketDimensionGeneration(
			4f,
			SubWorld.ZoneType.Radioactive,
			new List<SimHashes>
			{
				SimHashes.Chlorine,
				SimHashes.SolidCarbonDioxide,
				SimHashes.Vacuum,
				SimHashes.Snow,
				SimHashes.Ice,
				SimHashes.Wolframite,
				SimHashes.Vacuum,
				SimHashes.UraniumOre,
			},
			// very small blobs
			0.5f,
			0.5f
		),
		// rust biome
		new NoisePocketDimensionGeneration(
			4f,
			SubWorld.ZoneType.Rust,
			new List<SimHashes>
			{
				SimHashes.CarbonDioxide,
				SimHashes.SedimentaryRock,
				SimHashes.MaficRock,
				SimHashes.BleachStone,
				SimHashes.Vacuum,
				SimHashes.Rust,
				SimHashes.IronOre,
				SimHashes.Ethanol,
			},
			0.2f,
			0.15f
		),
		// Obsidian, niobium
		new NoisePocketDimensionGeneration(
			4f,
			SubWorld.ZoneType.Wasteland,
			new List<SimHashes>
			{
				SimHashes.Vacuum,
				SimHashes.Vacuum,
				SimHashes.Vacuum,
				SimHashes.Obsidian,
				SimHashes.Vacuum,
				SimHashes.Vacuum,
				SimHashes.Vacuum,
				SimHashes.Obsidian,
				SimHashes.Niobium,
			},
			0.1f,
			0.1f,
			"Mining3"
		),
	};

	/// <summary>
	///     Adds the specified generation config to the pool for pocket dimensions.
	/// </summary>
	/// <param name="config">The generation to add.</param>
	/// <seealso cref="BasePocketDimensionGeneration" />
	/// <seealso cref="TemplatePocketDimensionGeneration" />
	/// <seealso cref="NoisePocketDimensionGeneration" />
	/// <seealso cref="CustomPocketDimensionGeneration" />
	[PublicAPI]
	public static void AddGenerationConfig([NotNull] BasePocketDimensionGeneration config)
	{
		PocketDimensionSettings.Add(config);
	}

	/// <summary>
	///     Generates a new pocket dimension, with its outside portal placed at the specified cell.
	/// </summary>
	/// <param name="exteriorPortalCell">The cell to generate the outside portal.</param>
	/// <returns>The <see cref="GameObject" /> of the exterior door of the portal.</returns>
	[PublicAPI]
	[NotNull]
	public static GameObject GenerateDimension(int exteriorPortalCell)
	{
		var dimension = Util.KInstantiate(Assets.GetPrefab(PocketDimensionConfig.Id));
		dimension.SetActive(true);
		var world = WorldUtil.CreateWorldWithTemplate(
			dimension,
			PocketDimension.DimensionSize,
			PocketDimension.BorderTemplate,
			world =>
			{
				var settings = PocketDimensionSettings.Where(
						setting => (setting.RequiredSkillId == null) || Components.LiveMinionIdentities.Items.Any(
							minion => minion.TryGetComponent(out MinionResume resume) &&
									  resume.HasMasteredSkill(setting.RequiredSkillId)
						)
					)
					.GetRandom();

				var pocketDim = world.GetComponent<PocketDimension>();
				pocketDim.SetLifetime(
					settings.CyclesLifetime * Constants.SECONDS_PER_CYCLE,
					settings.CyclesLifetime * Constants.SECONDS_PER_CYCLE
				);

			#if DEBUG
				pocketDim.SetLifetime(
					60,
					settings.CyclesLifetime * Constants.SECONDS_PER_CYCLE
				);
			#endif

				// generate the world
				// natural tiles, prefabs, and possibly a nested dimension
				settings.Generate(world);

				// remove the old polygons and re-add the one we want.
				var overworldCell = Traverse.Create(world).Field<WorldDetailSave.OverworldCell>("overworldCell");

				var internalStart = world.WorldOffset + PocketDimension.InternalOffset -
									Vector2I.one;
				var internalEnd = world.WorldOffset +
								  PocketDimension.InternalOffset +
								  PocketDimension.InternalSize +
								  Vector2I.one;
				var verts = new List<Vector2>
				{
					internalStart,
					internalStart with { y = internalEnd.y },
					internalEnd,
					internalStart with { x = internalEnd.x },
				};
				overworldCell.Value.poly = new Polygon(verts);
				overworldCell.Value.zoneType = settings.ZoneType;

				// fill area around the world with void

				// below world
				for (var x = 0; x < PocketDimension.DimensionSize.x; x++)
				{
					for (var y = 0; y < 2; y++)
					{
						var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
						SimMessages.ReplaceElement(cell, SimHashes.Void, null, 0);
						Grid.Visible[cell] = 0;
						Grid.PreventFogOfWarReveal[cell] = true;
					}
				}

				// above world (4 tiles high for the surrounding no build zone)
				for (var x = 0; x < PocketDimension.DimensionSize.x; x++)
				{
					for (var y = PocketDimension.DimensionSize.y - 4;
						 y < PocketDimension.DimensionSize.y;
						 y++)
					{
						var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
						SimMessages.ReplaceElement(cell, SimHashes.Void, null, 0);
						Grid.Visible[cell] = 0;
						Grid.PreventFogOfWarReveal[cell] = true;
					}
				}

				// left of world
				for (var x = 0; x < 2; x++)
				{
					for (var y = 2; y < PocketDimension.DimensionSize.y - 2; y++)
					{
						var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
						SimMessages.ReplaceElement(cell, SimHashes.Void, null, 0);
						Grid.Visible[cell] = 0;
						Grid.PreventFogOfWarReveal[cell] = true;
					}
				}

				// right of world
				for (var x = PocketDimension.DimensionSize.x - 2;
					 x < PocketDimension.DimensionSize.x;
					 x++)
				{
					for (var y = 2; y < PocketDimension.DimensionSize.y - 2; y++)
					{
						var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
						SimMessages.ReplaceElement(cell, SimHashes.Void, null, 0);
						Grid.Visible[cell] = 0;
						Grid.PreventFogOfWarReveal[cell] = true;
					}
				}

				var portalPos = world.WorldOffset + PocketDimension.InternalOffset;

				// Make a ceiling above the door and open up the cells the door is on
				// Ceiling needed to ensure that falling tiles don't block the door
				SimMessages.ReplaceElement(
					Grid.PosToCell(portalPos + new Vector2I(0, 2)),
					SimHashes.Unobtanium,
					null,
					20_000f
				);
				SimMessages.ReplaceElement(Grid.PosToCell(portalPos), SimHashes.Vacuum, null, 0, 0);
				SimMessages.ReplaceElement(
					Grid.PosToCell(portalPos + new Vector2I(0, 1)),
					SimHashes.Vacuum,
					null,
					0,
					0
				);
			}
		);

		if (world == null)
		{
			// throw this instead of NRE-ing later, it will be handled by the event error handler
			throw new Exception("Unable to create a world for a pocket dimension");
		}

		// This setup happens immediately, before the world spawns, and before the template is placed
		var exteriorBuildingDef = Assets.GetBuildingDef(PocketDimensionExteriorPortalConfig.Id);
		var exteriorDoor = exteriorBuildingDef.Create(
			Grid.CellToPosCBC(exteriorPortalCell, exteriorBuildingDef.SceneLayer),
			null,
			new List<Tag> { SimHashes.Unobtanium.CreateTag() },
			null,
			0 + Constants.CELSIUS2KELVIN,
			exteriorBuildingDef.BuildingComplete
		);

		var portalPos = world.WorldOffset + PocketDimension.InternalOffset;
		var interiorBuildingDef = Assets.GetBuildingDef(PocketDimensionInteriorPortalConfig.Id);
		var interiorDoor = interiorBuildingDef.Create(
			new Vector3(portalPos.x, portalPos.y),
			null,
			new List<Tag> { SimHashes.Unobtanium.CreateTag() },
			null,
			0 + Constants.CELSIUS2KELVIN,
			interiorBuildingDef.BuildingComplete
		);

		var exteriorPortal = exteriorDoor.GetComponent<PocketDimensionExteriorPortal>();
		var interiorPortal = interiorDoor.GetComponent<PocketDimensionInteriorPortal>();

		exteriorPortal.CreatedWorldIdx = world.id;
		exteriorPortal.InteriorPortal = new Ref<PocketDimensionInteriorPortal>(interiorPortal);

		var exteriorPortalRef = new Ref<PocketDimensionExteriorPortal>(exteriorPortal);
		interiorPortal.ExteriorPortal = exteriorPortalRef;

		world.GetComponent<PocketDimension>().ExteriorPortal = exteriorPortalRef;

		// no light or rads
		world.sunlightFixedTrait = FIXEDTRAITS.SUNLIGHT.NAME.NONE;
		world.sunlight = FIXEDTRAITS.SUNLIGHT.NONE;
		world.cosmicRadiationFixedTrait = FIXEDTRAITS.COSMICRADIATION.NAME.NONE;
		world.cosmicRadiation = FIXEDTRAITS.COSMICRADIATION.NONE;

		return exteriorDoor;
	}
}
