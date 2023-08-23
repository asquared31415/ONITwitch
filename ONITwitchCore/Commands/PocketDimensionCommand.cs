using ONITwitch.Content;
using ONITwitch.Content.BuildingConfigs;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.Commands;

internal class PocketDimensionCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return DlcManager.FeatureClusterSpaceEnabled();
	}

	public override void Run(object data)
	{
		int startCell;
		if (Components.Telepads.Items.Count != 0)
		{
			startCell = Grid.PosToCell(Components.Telepads.Items.GetRandom());
			Log.Info($"Pocket dimension using start cell {startCell} from telepad");
		}
		else
		{
			var currentWorld = ClusterManager.Instance.activeWorld;
			Log.Info($"Pocket dimension using active world {currentWorld}");
			var midX = currentWorld.minimumBounds.x +
					   (currentWorld.maximumBounds.x - currentWorld.minimumBounds.x) / 2;
			var midY = currentWorld.minimumBounds.y +
					   (currentWorld.maximumBounds.y - currentWorld.minimumBounds.y) / 2;
			startCell = Grid.PosToCell(new Vector2(midX, midY));
			Log.Info($"Using cell {startCell} from active world");
		}

		var placeCell = GridUtil.FindCellOpenToBuilding(
			startCell,
			Assets.GetBuildingDef(PocketDimensionExteriorPortalConfig.Id)
		);
		Log.Info($"Found final cell {placeCell} from start cell {startCell}");
		var portalGo = PocketDimensionGenerator.GenerateDimension(placeCell);
		ToastManager.InstantiateToastWithGoTarget(
			STRINGS.ONITWITCH.TOASTS.POCKET_DIMENSION.TITLE,
			STRINGS.ONITWITCH.TOASTS.POCKET_DIMENSION.BODY,
			portalGo
		);
	}
}
