using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Cmps;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class GeyserPatches
{
	[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
	// ReSharper disable once InconsistentNaming
	private static class GeyserConfig_CreatePrefab_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(List<GameObject> __result)
		{
			foreach (var go in __result)
			{
				go.AddOrGet<TimedGeyserTuning>();
			}
		}
	}
}
