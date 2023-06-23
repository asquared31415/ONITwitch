using ONITwitch.Content;
using ONITwitch.Content.Buildings;
using ONITwitch.Toasts;
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

		var placeCell = GridUtil.FindCellOpenToBuilding(
			startCell,
			Assets.GetBuildingDef(PocketDimensionExteriorPortalConfig.Id)
		);
		var portalGo = PocketDimensionGenerator.GenerateDimension(placeCell);
		ToastManager.InstantiateToastWithGoTarget(
			STRINGS.ONITWITCH.TOASTS.POCKET_DIMENSION.TITLE,
			STRINGS.ONITWITCH.TOASTS.POCKET_DIMENSION.BODY,
			portalGo
		);
	}
}
