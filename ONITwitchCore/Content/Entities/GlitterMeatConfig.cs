using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore.Content.Entities;

[UsedImplicitly]
public class GlitterMeatConfig : IEntityConfig
{
	public const string Id = TwitchModInfo.ModPrefix + nameof(GlitterMeatConfig);

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateLooseEntity(
			Id,
			STRINGS.ITEMS.FOOD.ONITWITCH.GLITTERMEATCONFIG.NAME,
			STRINGS.ITEMS.FOOD.ONITWITCH.GLITTERMEATCONFIG.DESC,
			1f,
			false,
			Assets.GetAnim((HashedString) "creaturemeat_kanim"),
			"object",
			Grid.SceneLayer.Front,
			EntityTemplates.CollisionShape.RECTANGLE,
			0.8f,
			0.4f,
			true
		);
		EntityTemplates.ExtendEntityToFood(
			go,
			new EdiblesManager.FoodInfo(Id, "", 1600000f, 1, 255.15f, 277.15f, 4800f, true)
		);

		var sparkles = go.AddOrGet<SparkleFollower>();
		sparkles.NumSparkles = 1;
		sparkles.Offset = new Vector2(0, -0.4f);

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
