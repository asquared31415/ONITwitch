using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class GeyserPatches
{
	[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
	public static class GeyserConfig_CreatePrefab_Patch
	{
		[UsedImplicitly]
		public static void Postfix(List<GameObject> __result)
		{
			foreach (var go in __result)
			{
				go.AddOrGet<TimedGeyserTuning>();
			}
		}
	}
}
