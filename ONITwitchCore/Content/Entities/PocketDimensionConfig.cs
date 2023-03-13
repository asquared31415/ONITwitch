using JetBrains.Annotations;
using ONITwitchCore.Cmps.PocketDimension;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore.Content.Entities;

[UsedImplicitly]
internal class PocketDimensionConfig : IEntityConfig
{
	public const string Id = TwitchModInfo.ModPrefix + nameof(PocketDimensionConfig);

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateEntity(Id, "Pocket Dimension");
		var saveLoadRoot = go.AddOrGet<SaveLoadRoot>();
		saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
		saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
		go.AddOrGet<PocketDimensionClusterGridEntity>();
		go.AddOrGetDef<AlertStateManager.Def>();
		go.AddOrGet<PocketDimension>();

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
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
