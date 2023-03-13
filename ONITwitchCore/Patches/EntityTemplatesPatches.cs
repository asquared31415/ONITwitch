using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

// none of these are actually public APIs
#pragma warning disable CS1591

namespace ONITwitchCore.Patches;

/// <summary>
/// Not a public API
/// </summary>
public static class EntityTemplatesPatches
{
	[MustUseReturnValue]
	public static bool TryGetAdultFromEgg(Tag eggTag, out Tag adultTag)
	{
		if (EggToBabyMap.TryGetValue(eggTag, out var babyTag) && BabyToAdultMap.TryGetValue(babyTag, out adultTag))
		{
			return true;
		}

		adultTag = Tag.Invalid;
		return false;
	}

	private static readonly Dictionary<Tag, Tag> EggToBabyMap = new();
	private static readonly Dictionary<Tag, Tag> BabyToAdultMap = new();

	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToFertileCreature))]
	// ReSharper disable once InconsistentNaming
	public static class ExtendEntityToFertileCreature_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		public static void Prefix(string eggId, string baby_id)
		{
			EggToBabyMap[eggId] = baby_id;
		}
	}

	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBeingABaby))]
	// ReSharper disable once InconsistentNaming
	public static class ExtendEntityToBeingABaby_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		public static void Prefix(GameObject prefab, Tag adult_prefab_id)
		{
			if (prefab.TryGetComponent(out KPrefabID prefabID))
			{
				BabyToAdultMap[prefabID.PrefabTag] = adult_prefab_id;
			}
		}
	}
}
