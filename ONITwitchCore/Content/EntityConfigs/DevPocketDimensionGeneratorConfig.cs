using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.EntityConfigs;

// An entity used to generate a pocket dimension portal at its current location
[UsedImplicitly]
internal class DevPocketDimensionGeneratorConfig : IEntityConfig
{
	public const string Id = TwitchModInfo.ModPrefix + nameof(DevPocketDimensionGeneratorConfig);

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateBasicEntity(
			Id,
			STRINGS.ITEMS.ONITWITCH.DEV_POCKET_DIMENSION_GENERATOR.NAME,
			STRINGS.ITEMS.ONITWITCH.DEV_POCKET_DIMENSION_GENERATOR.DESC,
			1f,
			true,
			Assets.GetAnim("meallicegrain_kanim"),
			"object",
			Grid.SceneLayer.Front
		);
		return go;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		var cell = Grid.PosToCell(inst);
		PocketDimensionGenerator.GenerateDimension(cell);
		Object.Destroy(inst);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
