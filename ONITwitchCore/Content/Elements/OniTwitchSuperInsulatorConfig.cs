using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.Elements;

[UsedImplicitly]
internal class OniTwitchSuperInsulatorConfig : IOreConfig
{
	public SimHashes ElementID => TwitchSimHashes.OniTwitchSuperInsulator;

	public GameObject CreatePrefab()
	{
		var element = ElementLoader.FindElementByHash(ElementID);
		// create a dummy object so that when the game goes to spawn this prefab, nothing spawns
		var go = new GameObject(element.name);
		Object.DontDestroyOnLoad(go);
		// Don't be active or the destroy component will destroy the prefab.
		go.SetActive(false);

		var prefabID = go.AddOrGet<KPrefabID>();
		prefabID.PrefabTag = element.tag;
		prefabID.InitializeTags();

		// Needed to spawn the resource, even though it will be removed
		var primaryElement = go.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(ElementID);
		primaryElement.Mass = 1f;
		primaryElement.Temperature = element.defaultValues.temperature;

		go.AddOrGet<DeleteOnSpawn>();

		return go;
	}
}
