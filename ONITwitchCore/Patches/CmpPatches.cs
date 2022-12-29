using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class CmpPatches
{
	[HarmonyPatch]
	public static class AllToiletsPatch
	{
		[UsedImplicitly]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(
				typeof(OuthouseConfig),
				nameof(OuthouseConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(FlushToiletConfig),
				nameof(FlushToiletConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(WallToiletConfig),
				nameof(WallToiletConfig.DoPostConfigureComplete)
			);
		}

		[UsedImplicitly]
		public static void Postfix(GameObject go)
		{
			go.AddOrGet<ToiletsExt>();
		}
	}

	[HarmonyPatch(typeof(InsulationTileConfig), nameof(InsulationTileConfig.DoPostConfigureComplete))]
	public static class InsulatedTilesPatch
	{
		[UsedImplicitly]
		public static void Postfix(GameObject go)
		{
			go.AddOrGet<InsulatedTileExt>();
		}
	}
}
