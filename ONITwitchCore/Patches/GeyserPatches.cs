using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using UnityEngine;

namespace ONITwitchCore.Patches;

internal static class GeyserPatches
{
	[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
	private static class GeyserConfig_CreatePrefab_Patch
	{
		[UsedImplicitly]
		private static void Postfix(List<GameObject> __result)
		{
			foreach (var go in __result)
			{
				go.AddOrGet<TimedGeyserTuning>();
			}
		}
	}
}
