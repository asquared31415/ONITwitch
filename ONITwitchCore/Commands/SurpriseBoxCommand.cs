using System.Collections.Generic;
using ONITwitchCore.Content.Buildings;
using ONITwitchCore.Toasts;
using ONITwitchLib.Utils;

namespace ONITwitchCore.Commands;

public class SurpriseBoxCommand : CommandBase
{
	public override void Run(object data)
	{
		int startCell;
		if (Components.Telepads.Count > 0)
		{
			startCell = Grid.PosToCell(Components.Telepads.Items.GetRandom());
		}
		else if (Components.LiveMinionIdentities.Count > 0)
		{
			startCell = Grid.PosToCell(Components.LiveMinionIdentities.Items.GetRandom());
		}
		else
		{
			Debug.LogWarning("[Twitch Integration] Unable to spawn a Surprise Box, no telepads or live minions");
			return;
		}

		var spawnCell = GridUtil.FindCellOpenToBuilding(startCell, new[] { CellOffset.none });
		var surpriseDef = Assets.GetBuildingDef(SurpriseBoxConfig.Id);
		var box = surpriseDef.Build(
			spawnCell,
			Orientation.Neutral,
			null,
			new List<Tag> { SimHashes.SandStone.CreateTag() },
			273.15f,
			false
		);

		ToastManager.InstantiateToastWithGoTarget(
			"Surprise Box",
			"A Surprise Box has been created. I wonder what could be inside...",
			box
		);
	}
}
