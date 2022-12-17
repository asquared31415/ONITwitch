using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class SurpriseTilePatches
{
	[HarmonyPatch]
	public static class EntityPatch
	{
		[UsedImplicitly]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(DustCometConfig), nameof(DustCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(FoodCometConfig), nameof(FoodCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(GoldCometConfig), nameof(GoldCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(IronCometConfig), nameof(IronCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(RockCometConfig), nameof(RockCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(CopperCometConfig), nameof(CopperCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(GassyMooCometConfig), nameof(GassyMooCometConfig.CreatePrefab));
			yield return AccessTools.Method(typeof(FullereneCometConfig), nameof(FullereneCometConfig.CreatePrefab));
			yield return AccessTools.Method(
				typeof(NuclearWasteCometConfig),
				nameof(NuclearWasteCometConfig.CreatePrefab)
			);
			
			// NOTE: Satellite comet explicitly not included here, it's very very very dangerous
		}

		[UsedImplicitly]
		public static void Postfix(GameObject __result)
		{
			__result.AddOrGet<EnableSurpriseBoxMarker>();
		}
	}
}
