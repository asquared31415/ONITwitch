using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;

namespace ONITwitch.Commands;

internal class SpawnPrefabCommand : CommandBase
{
	public override bool Condition(object data)
	{
		var prefabId = (string) data;
		return Assets.TryGetPrefab(prefabId) != null;
	}

	public override void Run(object data)
	{
		var prefabId = (string) data;
		var prefab = Assets.GetPrefab(prefabId);
		var cell = GridUtil.NearestEmptyCell(PosUtil.RandomCellNearMouse());
		var go = GameUtil.KInstantiate(
			prefab,
			Grid.CellToPosCBC(cell, Grid.SceneLayer.Move),
			Grid.SceneLayer.Move
		);
		if (go != null)
		{
			go.SetActive(true);
			ToastManager.InstantiateToastWithGoTarget(
				STRINGS.ONITWITCH.TOASTS.SPAWN_PREFAB.TITLE,
				string.Format(
					STRINGS.ONITWITCH.TOASTS.SPAWN_PREFAB.BODY_FORMAT,
					Util.StripTextFormatting(prefab.GetProperName())
				),
				go
			);
		}
		else
		{
			Log.Warn($"Unable to spawn prefab {prefabId}");
		}
	}
}
