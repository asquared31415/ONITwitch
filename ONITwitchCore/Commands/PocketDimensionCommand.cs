using HarmonyLib;
using Klei;
using ONITwitchCore.Content;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitchCore.Commands;

public class PocketDimensionCommand : CommandBase
{
	private static readonly AccessTools.FieldRef<WorldContainer, WorldDetailSave.OverworldCell> OverworldCellAccess =
		AccessTools.FieldRefAccess<WorldContainer, WorldDetailSave.OverworldCell>("overworldCell");

	public override void Run(object data)
	{
		int startCell;
		if (Components.Telepads.Items.Count != 0)
		{
			startCell = Grid.PosToCell(Components.Telepads.Items.GetRandom());
		}
		else
		{
			var currentWorld = ClusterManager.Instance.activeWorld;
			var midX = currentWorld.minimumBounds.x +
					   (currentWorld.maximumBounds.x - currentWorld.minimumBounds.x) / 2;
			var midY = currentWorld.minimumBounds.y +
					   (currentWorld.maximumBounds.y - currentWorld.minimumBounds.y) / 2;
			startCell = Grid.PosToCell(new Vector2(midX, midY));
		}

		var placeCell = GridUtil.FindCellOpenToBuilding(startCell, new[] { CellOffset.none, new CellOffset(0, 1) });
		PocketDimensionGenerator.GenerateDimension(placeCell);
	}
}
