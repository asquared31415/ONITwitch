using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

// none of these are actually public APIs
#pragma warning disable CS1591

namespace ONITwitch.Patches;

public static class EntityTemplatesPatches
{
	private static readonly Dictionary<Tag, Tag> EggToBabyMap = new();
	private static readonly Dictionary<Tag, Tag> BabyToAdultMap = new();

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

	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToFertileCreature), [
		typeof(GameObject),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(float),
		typeof(string),
		typeof(float),
		typeof(float),
		typeof(List<FertilityMonitor.BreedingChance>),
		typeof(string[]),
		typeof(int),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(float),
		typeof(bool),
	])]
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

	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBeingABaby), [
		typeof(GameObject),
		typeof(Tag),
		typeof(string),
		typeof(bool),
		typeof(float),
	])]
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
