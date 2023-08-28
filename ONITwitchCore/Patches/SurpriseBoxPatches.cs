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
			// Does nothing, has no contents, and cannot be emptied to remove it.
			yield return AccessTools.Method(typeof(DebrisPayloadConfig), nameof(DebrisPayloadConfig.CreatePrefab));
		}

		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(GameObject __result)
		{
			__result.AddTag(ExtraTags.OniTwitchSurpriseBoxForceDisabled);
		}
	}


	[HarmonyPatch]
	private static class DisabledMultiEntityPatches
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

	// Don't allow disabled elements to spawn in the surprise box (they can crash when pinned, and are useless).
	// Also don't let Neutrollium spawn, it's similarly useless and can't even be stored.
	[HarmonyPatch(typeof(GeneratedOre), nameof(GeneratedOre.LoadGeneratedOre), typeof(List<Type>))]
	private static class DisabledElementPatch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			foreach (var go in Assets.Prefabs)
			{
				if (go.TryGetComponent(out PrimaryElement primaryElement))
				{
					if (primaryElement.Element.disabled ||
						(primaryElement.ElementID == TwitchSimHashes.OniTwitchIndestructibleElement))
					{
						go.AddTag(ExtraTags.OniTwitchSurpriseBoxForceDisabled);
					}
				}
			}
		}
	}
}
