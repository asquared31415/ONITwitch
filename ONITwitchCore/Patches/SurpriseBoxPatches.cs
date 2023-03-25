using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class SurpriseBoxPatches
{
	[HarmonyPatch]
	private static class EnabledEntityPatches
	{
		[UsedImplicitly]
		private static IEnumerable<MethodBase> TargetMethods()
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
		// ReSharper disable once InconsistentNaming
		private static void Postfix(GameObject __result)
		{
			__result.AddTag(ExtraTags.SurpriseBoxForceEnabled);
		}
	}

	[HarmonyPatch]
	private static class DisabledEntityPatches
	{
		[UsedImplicitly]
		private static IEnumerable<MethodBase> TargetMethods()
		{
			// doesnt like being spawned manually without special care
			yield return AccessTools.Method(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab));
			// unimplemented and crashy
			yield return AccessTools.Method(typeof(ShockwormConfig), nameof(ShockwormConfig.CreatePrefab));
		}

		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(GameObject __result)
		{
			__result.AddTag(ExtraTags.SurpriseBoxForceDisabled);
		}
	}
}
