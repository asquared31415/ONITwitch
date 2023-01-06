using ONITwitchCore.Content.Entities;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class SurpriseBoxCommand : CommandBase
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
			Debug.LogWarning("[Twitch Integration] Unable to spawn a Surprise Box, no telepads or live minions");
			return;
		}

		var boxPrefab = Assets.GetPrefab(SurpriseBoxConfig.Id);
		var box = GameUtil.KInstantiate(
			boxPrefab,
			Grid.CellToPosCBC(spawnCell, Grid.SceneLayer.Front),
			Grid.SceneLayer.Front
		);
		box.SetActive(true);

		ToastManager.InstantiateToastWithGoTarget(
			"Surprise Box",
			"A Surprise Box has been created. I wonder what could be inside...",
			box
		);
	}
}
