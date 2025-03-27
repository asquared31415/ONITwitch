using JetBrains.Annotations;
using ONITwitch.Content.Cmps.PocketDimension;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.EntityConfigs;

[UsedImplicitly]
internal class PocketDimensionConfig : IEntityConfig, IHasDlcRestrictions
{
	public static readonly string Id = ExtraTags.PocketDimensionEntityTag.Name;

	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateEntity(Id, "Pocket Dimension");
		var saveLoadRoot = go.AddOrGet<SaveLoadRoot>();
		saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
		saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
		go.AddOrGet<PocketDimensionClusterGridEntity>();
		go.AddOrGetDef<AlertStateManager.Def>();
		go.AddOrGet<PocketDimension>();

		var component = go.AddOrGet<KPrefabID>();
		component.AddTag(ExtraTags.PocketDimensionEntityTag);

		return go;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
	public string[] GetForbiddenDlcIds() => null;
	public string[] GetDlcIds()
	{
		return null;
	}
}
