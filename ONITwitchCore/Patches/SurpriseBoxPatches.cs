using System;
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
			// NOTE: Satellite comet explicitly not included here, it's very very very dangerous
			var types = new[]
			{
				// base game comets
				typeof(DustCometConfig),
				typeof(GoldCometConfig),
				typeof(IronCometConfig),
				typeof(RockCometConfig),
				typeof(CopperCometConfig),

				// DLC comets
				// note: The actual prefab is not created if DLC is not enabled, so it can't be chosen
				typeof(NuclearWasteCometConfig),
				typeof(AlgaeCometConfig),
				typeof(BleachStoneCometConfig),
				typeof(FullereneCometConfig),
				typeof(OxyliteCometConfig),
				typeof(PhosphoricCometConfig),
				typeof(SlimeCometConfig),
				typeof(SnowballCometConfig),
				typeof(UraniumCometConfig),

				// extra comets
				typeof(FoodCometConfig),
				typeof(GassyMooCometConfig),
			};

			foreach (var type in types)
			{
				yield return AccessTools.Method(type, "CreatePrefab");
			}
		}

		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(GameObject __result)
		{
			__result.AddTag(ExtraTags.OniTwitchSurpriseBoxForceEnabled);
		}
	}

	[HarmonyPatch]
	private static class DisabledSingleEntityPatches
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
			__result.AddTag(ExtraTags.OniTwitchSurpriseBoxForceDisabled);
		}
	}


	[HarmonyPatch]
	private static class DisabledSingleMultiPatches
	{
		[UsedImplicitly]
		private static IEnumerable<MethodBase> TargetMethods()
		{
			// artifacts are not interesting
			yield return AccessTools.Method(typeof(ArtifactConfig), nameof(ArtifactConfig.CreatePrefabs));
		}

		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(List<GameObject> __result)
		{
			foreach (var gameObject in __result)
			{
				gameObject.AddTag(ExtraTags.OniTwitchSurpriseBoxForceDisabled);
			}
		}
	}

	[HarmonyPatch(typeof(GeneratedOre), nameof(GeneratedOre.LoadGeneratedOre), typeof(List<Type>))]
	private static class DisabledElementPatch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			foreach (var go in Assets.GetPrefabsWithComponent<ElementChunk>())
			{
				go.AddTag(ExtraTags.OniTwitchSurpriseBoxForceDisabled);
			}
		}
	}
}
