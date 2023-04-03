using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Cmps;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class CmpPatches
{
	[HarmonyPatch]
	private static class AllToiletsPatch
	{
		[UsedImplicitly]
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(
				typeof(OuthouseConfig),
				nameof(OuthouseConfig.ConfigureBuildingTemplate)
			);
			yield return AccessTools.Method(
				typeof(FlushToiletConfig),
				nameof(FlushToiletConfig.ConfigureBuildingTemplate)
			);
			yield return AccessTools.Method(
				typeof(WallToiletConfig),
				nameof(WallToiletConfig.ConfigureBuildingTemplate)
			);
		}

		[UsedImplicitly]
		private static void Postfix(GameObject go)
		{
			go.AddOrGet<OniTwitchToiletsExt>();
		}
	}

	[HarmonyPatch(typeof(InsulationTileConfig), nameof(InsulationTileConfig.DoPostConfigureComplete))]
	private static class InsulatedTilesPatch
	{
		[UsedImplicitly]
		private static void Postfix(GameObject go)
		{
			go.AddOrGet<OniTwitchInsulatedTileExt>();
		}
	}
}
