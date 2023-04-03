using System.Collections.Generic;
using System.Linq;
using ONITwitch.Cmps;
using ONITwitch.Patches;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;

namespace ONITwitch.Commands;

internal class RainPrefabCommand : CommandBase
{
	private const float TimePerItem = 0.1f;

	public override bool Condition(object o)
	{
		var data = (IDictionary<string, object>) o;
		var prefab = (string) data["PrefabId"];
		return Assets.TryGetPrefab(prefab) != null;
	}

	public override void Run(object o)
	{
		var data = (IDictionary<string, object>) o;
		var prefabId = (string) data["PrefabId"];
		var count = (int) (double) data["Count"];

		var prefabIds = new List<(Tag Tag, float Weight)> { (prefabId, 1) };

		var prefab = Assets.GetPrefab(prefabId);
		if (prefab == null)
		{
			Log.Warn($"Cannot rain missing prefab {prefabId}");
			return;
		}

		// if the prefab has morphs, use them
		var fertilityDef = prefab.GetDef<FertilityMonitor.Def>();
		if (fertilityDef != null)
		{
			prefabIds = ExtractMorphs(fertilityDef.initialBreedingWeights);
		}

		var rainPrefab = Game.Instance.gameObject.AddOrGet<OniTwitchRainPrefab>();
		rainPrefab.Initialize(TimePerItem, count, prefabIds);

		ToastManager.InstantiateToast(
			STRINGS.ONITWITCH.TOASTS.RAIN_PREFAB.TITLE,
			string.Format(
				STRINGS.ONITWITCH.TOASTS.RAIN_PREFAB.BODY_FORMAT,
				prefab.GetProperName()
			)
		);
	}


	private static List<(Tag Tag, float Weight)> ExtractMorphs(
		IEnumerable<FertilityMonitor.BreedingChance> breedingChances
	)
	{
		var options = breedingChances.Select(
				chance => EntityTemplatesPatches.TryGetAdultFromEgg(chance.egg, out var adultTag)
					? (adultTag, chance.weight)
					: (Tag.Invalid, 0)
			)
			.Where(pair => pair.adultTag.IsValid && (Assets.TryGetPrefab(pair.adultTag) != null))
			.ToList();
		var totalWeight = options.Sum(pair => pair.weight);
		for (var idx = 0; idx < options.Count; idx++)
		{
			var old = options[idx];
			options[idx] = old with { weight = old.weight / totalWeight };
		}

		return options;
	}
}
