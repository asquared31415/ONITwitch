using ONITwitchCore.Cmps;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore.Content.Entities;

public class SurpriseBoxConfig : IEntityConfig
{
	public const string Id = TwitchModInfo.ModPrefix + nameof(SurpriseBoxConfig);
	public const string Anim = "twitch_surprise_box_kanim";

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateLooseEntity(
			Id,
			TIStrings.STRINGS.ITEMS.ONITWITCH.SURPRISEBOXCONFIG.NAME,
			TIStrings.STRINGS.ITEMS.ONITWITCH.SURPRISEBOXCONFIG.DESC,
			100f,
			true,
			Assets.GetAnim(Anim),
			"closed",
			// want it to be below ore
			Grid.SceneLayer.BuildingFront,
			EntityTemplates.CollisionShape.RECTANGLE,
			1f,
			1.1f,
			true
		);

		go.AddOrGet<SurpriseBox>();

		return go;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}
}
