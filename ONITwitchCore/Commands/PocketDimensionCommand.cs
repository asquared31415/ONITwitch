using System.Collections.Generic;
using Delaunay.Geo;
using HarmonyLib;
using Klei;
using ONITwitchCore.Cmps.PocketDimension;
using ONITwitchCore.Content.Buildings;
using ONITwitchCore.Content.Entities;
using ONITwitchLib;
using ProcGen;
using UnityEngine;

namespace ONITwitchCore.Commands;

public class PocketDimensionCommand : CommandBase
{
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
					// TODO: fill randomly
				}

				var verts = new List<Vector2>
				{
					world.WorldOffset,
					world.WorldOffset with { y = world.WorldOffset.y + PocketDimension.DimensionSize.y },
					world.WorldOffset + PocketDimension.DimensionSize,
					world.WorldOffset with { x = world.WorldOffset.x + PocketDimension.DimensionSize.x },
				};
				var cell = Traverse.Create(world).Field<WorldDetailSave.OverworldCell>("overworldCell").Value;
				cell.poly = new Polygon(verts);
				cell.zoneType = SubWorld.ZoneType.Barren;

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
}
