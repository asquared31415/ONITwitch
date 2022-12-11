using ONITwitchLib;
using ONITwitchLib.Utils;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

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
			// This can choose from the fancy suits or the player's unlocked skins
			var randomFacade = Db.GetEquippableFacades().resources.GetRandom();
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
