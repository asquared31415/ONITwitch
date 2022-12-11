using System.Collections.Generic;
using System.Linq;
using Delaunay.Geo;
using HarmonyLib;
using JetBrains.Annotations;
using Klei;
using ONITwitchCore.Cmps.PocketDimension;
using ONITwitchCore.Content.Buildings;
using ONITwitchCore.Content.Entities;
using ONITwitchLib;
using ProcGen;
using TUNING;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONITwitchCore.Commands;

public class PocketDimensionCommand : CommandBase
{
	private static readonly AccessTools.FieldRef<WorldContainer, WorldDetailSave.OverworldCell> OverworldCellAccess =
		AccessTools.FieldRefAccess<WorldContainer, WorldDetailSave.OverworldCell>("overworldCell");

	public override void Run(object data)
	{
		var dimension = Util.KInstantiate(Assets.GetPrefab(PocketDimensionConfig.Id));
		dimension.SetActive(true);
		var world = WorldUtils.CreateWorldWithTemplate(
			dimension,
			PocketDimension.DimensionSize,
			PocketDimension.BorderTemplate,
			world =>
			{
				const int percentMeep = 5;
				if (Random.Range(0, 100 / percentMeep) == 0)
				{
					world.PlaceInteriorTemplate(PocketDimension.MeepTemplate, () => { });
				}
				else
				{
					// TODO: the logic
					FillWithNoise(
						world.WorldOffset + PocketDimension.InternalOffset,
						PocketDimension.InternalSize,
						new List<SimHashes>()
						{
							SimHashes.SandStone,
							SimHashes.SandStone,
							SimHashes.SandStone,
							SimHashes.Algae,
							SimHashes.Fertilizer,
							SimHashes.Dirt,
						}
					);
				}

				// remove the old polygons and re-add the one we want.
				var clusterSave = SaveLoader.Instance.clusterDetailSave;
				clusterSave.overworldCells.Remove(OverworldCellAccess(world));

				var internalStart = world.WorldOffset + PocketDimension.InternalOffset - Vector2I.one;
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
				ref var overworldCell = ref OverworldCellAccess(world);
				overworldCell = new WorldDetailSave.OverworldCell
				{
					poly = new Polygon(verts),
					zoneType = SubWorld.ZoneType.Barren,
				};

				clusterSave.overworldCells.Add(overworldCell);

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
					for (var y = PocketDimension.DimensionSize.y - 4; y < PocketDimension.DimensionSize.y; y++)
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
				for (var x = PocketDimension.DimensionSize.x - 2; x < PocketDimension.DimensionSize.x; x++)
				{
					for (var y = 2; y < PocketDimension.DimensionSize.y - 2; y++)
					{
						var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
						SimMessages.ReplaceElement(cell, SimHashes.Void, null, 0);
						Grid.Visible[cell] = 0;
						Grid.PreventFogOfWarReveal[cell] = true;
					}
				}
				
				// no light or rads
				world.sunlightFixedTrait = FIXEDTRAITS.SUNLIGHT.NAME.NONE;
				world.sunlight = FIXEDTRAITS.SUNLIGHT.NONE;
				world.cosmicRadiationFixedTrait = FIXEDTRAITS.COSMICRADIATION.NAME.NONE;
				world.cosmicRadiation = FIXEDTRAITS.COSMICRADIATION.NONE;

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

		// Find a valid location for the building
		static bool IsValidPortalCell(int cell)
		{
			var thisCellOkay = !Grid.IsSolidCell(cell) && Grid.IsValidBuildingCell(cell) &&
							   (Grid.Objects[cell, (int) ObjectLayer.Building] == null);
			var cellAbove = Grid.CellAbove(cell);
			var cellAboveOkay = !Grid.IsSolidCell(cellAbove) && Grid.IsValidBuildingCell(cellAbove) &&
								(Grid.Objects[cellAbove, (int) ObjectLayer.Building] == null);

			return thisCellOkay && cellAboveOkay;
		}

		int startCell;
		if (Components.Telepads.Items.Count == 0)
		{
			var currentWorld = ClusterManager.Instance.activeWorld;
			var midX = currentWorld.minimumBounds.x +
					   (currentWorld.maximumBounds.x - currentWorld.minimumBounds.x) / 2;
			var midY = currentWorld.minimumBounds.y +
					   (currentWorld.maximumBounds.y - currentWorld.minimumBounds.y) / 2;
			startCell = Grid.PosToCell(new Vector2(midX, midY));
		}
		else
		{
			var telepad = Components.Telepads.Items.GetRandom();
			startCell = Grid.PosToCell(telepad);
		}

		var foundCell = GameUtil.FloodFillFind<object>(
			(cell, _) => IsValidPortalCell(cell),
			null,
			startCell,
			1_000,
			false,
			false
		);

		if (foundCell == -1)
		{
			Debug.LogWarning(
				$"[Twitch Integration] Unable to find a cell for the pocket dimension portal starting at {startCell}"
			);
			foundCell = startCell;
		}

		var exteriorBuildingDef = Assets.GetBuildingDef(PocketDimensionExteriorPortalConfig.Id);
		var exteriorDoor = exteriorBuildingDef.Create(
			Grid.CellToPosCBC(foundCell, exteriorBuildingDef.SceneLayer),
			null,
			new List<Tag> { SimHashes.Unobtanium.CreateTag() },
			null,
			0.0001f,
			exteriorBuildingDef.BuildingComplete
		);

		// place interior door at known position inside world
		var portalPos = world.WorldOffset + PocketDimension.InternalOffset;

		var interiorBuildingDef = Assets.GetBuildingDef(PocketDimensionInteriorPortalConfig.Id);
		var interiorDoor = interiorBuildingDef.Create(
			new Vector3(portalPos.x, portalPos.y),
			null,
			new List<Tag> { SimHashes.Unobtanium.CreateTag() },
			null,
			0.0001f,
			interiorBuildingDef.BuildingComplete
		);

		var exteriorPortal = exteriorDoor.GetComponent<PocketDimensionExteriorPortal>();
		var interiorPortal = interiorDoor.GetComponent<PocketDimensionInteriorPortal>();

		exteriorPortal.CreatedWorldIdx = world.id;
		exteriorPortal.InteriorPortal = new Ref<PocketDimensionInteriorPortal>(interiorPortal);

		var exteriorPortalRef = new Ref<PocketDimensionExteriorPortal>(exteriorPortal);
		interiorPortal.ExteriorPortal = exteriorPortalRef;

		world.GetComponent<PocketDimension>().ExteriorPortal = exteriorPortalRef;
	}

	public static void FillWithNoise(
		Vector2I offset,
		Vector2I size,
		[NotNull] List<SimHashes> hashes,
		float seed = float.NaN
	)
	{
		if (float.IsNaN(seed))
		{
			seed = Time.realtimeSinceStartup;
		}

		var numElements = hashes.Count;
		var elements = hashes.Select(ElementLoader.FindElementByHash).ToList();

		var maxX = offset.x + size.x;
		for (var x = offset.x; x < maxX; x++)
		{
			var maxY = offset.y + size.y;
			for (var y = offset.y; y < maxY; y++)
			{
				// lower = more stretched out along the corresponding axis
				const float xFrequency = 0.05f;
				const float yFrequency = 0.15f;
				// Transform from (-1,1) to (0,1)
				var val = (PerlinSimplexNoise.noise(x * xFrequency, y * yFrequency, seed) + 1) / 2;
				var idx = (int) Mathf.Floor(val * numElements);
				var defaultValues = elements[idx].defaultValues;
				var mass = elements[idx].state switch
				{
					Element.State.Vacuum => 0f,
					Element.State.Gas => 5f,
					Element.State.Liquid => 750f,
					Element.State.Solid => 2000f,
					_ => 0f,
				};
				SimMessages.ReplaceAndDisplaceElement(
					Grid.XYToCell(x, y),
					hashes[idx],
					null,
					mass,
					defaultValues.temperature
				);
			}
		}
	}
}
