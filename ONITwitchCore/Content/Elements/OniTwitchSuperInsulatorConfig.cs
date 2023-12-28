using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.Elements;

[UsedImplicitly]
internal class OniTwitchSuperInsulatorConfig : IOreConfig
{
	public SimHashes ElementID => TwitchSimHashes.OniTwitchSuperInsulator;

	// Prefab that destroys itself on spawn.
	public GameObject CreatePrefab()
	{
		var go = EntityTemplates.CreateSolidOreEntity(ElementID);
		// Don't be active or the destroy component will destroy the prefab.
		go.SetActive(false);

		go.AddOrGet<DeleteOnSpawn>();

		return go;
	}
}
