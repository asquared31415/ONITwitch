using ONITwitch.Content.EntityConfigs;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;

namespace ONITwitch.Commands;

internal class SurpriseBoxCommand : CommandBase
{
	public override void Run(object data)
	{
		int spawnCell;
		if (Components.Telepads.Count > 0)
		{
			spawnCell = Grid.CellAbove(Grid.PosToCell(Components.Telepads.Items.GetRandom()));
		}
		else if (Components.LiveMinionIdentities.Count > 0)
		{
			spawnCell = Grid.PosToCell(Components.LiveMinionIdentities.Items.GetRandom());
		}
		else
		{
			Log.Warn("Unable to spawn a Surprise Box, no telepads or live minions");
			return;
		}

		var boxPrefab = Assets.GetPrefab(SurpriseBoxConfig.Id);
		var box = GameUtil.KInstantiate(
			boxPrefab,
			Grid.CellToPosCBC(spawnCell, Grid.SceneLayer.Front),
			Grid.SceneLayer.Front
		);
		box.SetActive(true);

		// get an angle at least 30 degrees above ground
		var velocity = 5 * RandomUtil.OnUnitCircleInRange(30, 150);
		GameComps.Fallers.Add(box, velocity);

		ToastManager.InstantiateToastWithGoTarget(
			STRINGS.ONITWITCH.TOASTS.SURPRISE_BOX.TITLE,
			STRINGS.ONITWITCH.TOASTS.SURPRISE_BOX.BODY,
			box
		);
	}
}
