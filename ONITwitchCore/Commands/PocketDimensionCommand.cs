using HarmonyLib;
using Klei;
using ONITwitchCore.Content;
using UnityEngine;

namespace ONITwitchCore.Commands;

public class PocketDimensionCommand : CommandBase
{
	private static readonly AccessTools.FieldRef<WorldContainer, WorldDetailSave.OverworldCell> OverworldCellAccess =
		AccessTools.FieldRefAccess<WorldContainer, WorldDetailSave.OverworldCell>("overworldCell");

	public override void Run(object data)
	{
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

		PocketDimensionGeneration.GenerateDimension(foundCell);
	}
}
