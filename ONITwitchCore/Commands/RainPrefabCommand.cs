using System;
using System.Collections.Generic;
using System.Linq;
using ONITwitchCore.Cmps;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class RainPrefabCommand : CommandBase
{
	private const float TimePerItem = 0.1f;

	public override bool Condition(object o)
	{
		var data = (IDictionary<string, object>) o;
		var prefab = (string) data["PrefabId"];
		return Assets.TryGetPrefab(prefab) != null;
	}

	private static readonly List<string> SlicksterMorphs = ExtractMorphs(OilFloaterTuning.EGG_CHANCES_BASE);
	private static readonly List<string> PacuMorphs = ExtractMorphs(PacuTuning.EGG_CHANCES_BASE);

	public override void Run(object o)
	{
		var data = (IDictionary<string, object>) o;
		var prefab = (string) data["PrefabId"];
		var count = (int) (double) data["Count"];

		var prefabs = new List<string> { prefab };

		// certain prefabs are actually a collection
		switch (prefab)
		{
			case OilFloaterConfig.ID:
			{
				prefabs = SlicksterMorphs;
				break;
			}
			case PacuConfig.ID:
			{
				prefabs = PacuMorphs;
				break;
			}
		}

		var rainPrefab = Game.Instance.gameObject.AddOrGet<RainPrefab>();
		rainPrefab.Initialize(TimePerItem, count, prefabs);

		ToastManager.InstantiateToast(
			"Rain",
			$"A rain of {Util.StripTextFormatting(Assets.GetPrefab(prefabs.First()).GetProperName())} is starting!"
		);
	}


	private static List<string> ExtractMorphs(IEnumerable<FertilityMonitor.BreedingChance> breedingChances)
	{
		return breedingChances.Select(
				chance =>
				{
					var eggTag = chance.egg.Name;
					return eggTag.EndsWith("Egg")
						? eggTag.Substring(0, eggTag.LastIndexOf("Egg", StringComparison.Ordinal))
						: null;
				}
			)
			.Where(prefabId => !prefabId.IsNullOrWhiteSpace())
			.ToList();
	}
}
