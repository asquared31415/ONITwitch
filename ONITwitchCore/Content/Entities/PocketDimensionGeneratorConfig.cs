using UnityEngine;

namespace ONITwitchCore.Content.Entities;

// An entity used to generate a pocket dimension portal at its current location
public class PocketDimensionGeneratorConfig : IEntityConfig
{
	public const string Id = "ONITwitch.PocketDimensionGenerator";

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateBasicEntity(
			Id,
			"Pocket Dimension Generator",
			"Generates a pocket dimension",
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
		PocketDimensionGeneration.GenerateDimension(cell);
		Object.Destroy(inst);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
