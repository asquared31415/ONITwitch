using ONITwitchCore.Toasts;
using ONITwitchLib;

namespace ONITwitchCore.Commands;

public class SnazzySuitCommand : CommandBase
{
	public override void Run(object data)
	{
		var prefab = Assets.GetPrefab(CustomClothingConfig.ID);
		var position = GridUtil.NearestEmptyCell(PosUtil.RandomCellNearMouse());
		var sceneLayer = Grid.SceneLayer.Front;
		if (prefab.TryGetComponent<KBatchedAnimController>(out var kbac))
		{
			sceneLayer = kbac.sceneLayer;
		}

		var go = GameUtil.KInstantiate(
			prefab,
			Grid.CellToPos(position, 0.01f, 0.01f, Grid.GetLayerZ(sceneLayer)),
			sceneLayer
		);
		if (go != null)
		{
			go.SetActive(true);

			// Add a random facade to the clothing from all loaded facades
			// This chooses one of the pretty new fancy suits
			var randomFacade = Db.Get().EquippableFacades.resources.GetRandom();
			EquippableFacade.AddFacadeToEquippable(go.GetComponent<Equippable>(), randomFacade.Id);

			ToastManager.InstantiateToastWithGoTarget(
				"Spawning Object",
				$"A new {Util.StripTextFormatting(prefab.GetProperName())} has been created",
				go
			);
		}
		else
		{
			Debug.LogWarning($"[Twitch Integration] Unable to spawn prefab {CustomClothingConfig.ID}");
		}
	}
}
