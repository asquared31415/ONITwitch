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
	public static class AllFloorTileConfigPatch
	{
		[UsedImplicitly]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(BunkerTileConfig), nameof(BunkerTileConfig.DoPostConfigureComplete));
			yield return AccessTools.Method(typeof(CarpetTileConfig), nameof(CarpetTileConfig.DoPostConfigureComplete));
			yield return AccessTools.Method(
				typeof(GasPermeableMembraneConfig),
				nameof(GasPermeableMembraneConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(typeof(GlassTileConfig), nameof(GlassTileConfig.DoPostConfigureComplete));
			yield return AccessTools.Method(
				typeof(InsulationTileConfig),
				nameof(InsulationTileConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(typeof(MeshTileConfig), nameof(MeshTileConfig.DoPostConfigureComplete));
			yield return AccessTools.Method(typeof(MetalTileConfig), nameof(MetalTileConfig.DoPostConfigureComplete));
			yield return AccessTools.Method(
				typeof(PlasticTileConfig),
				nameof(PlasticTileConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketEnvelopeWindowTileConfig),
				nameof(RocketEnvelopeWindowTileConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketInteriorGasInputPortConfig),
				nameof(RocketInteriorGasInputPortConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketInteriorGasOutputPortConfig),
				nameof(RocketInteriorGasOutputPortConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketInteriorLiquidInputPortConfig),
				nameof(RocketInteriorLiquidInputPortConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketInteriorLiquidOutputPortConfig),
				nameof(RocketInteriorLiquidOutputPortConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(
				typeof(RocketWallTileConfig),
				nameof(RocketWallTileConfig.DoPostConfigureComplete)
			);
			yield return AccessTools.Method(typeof(TileConfig), nameof(TileConfig.DoPostConfigureComplete));
		}

		[UsedImplicitly]
		public static void Postfix(GameObject go)
		{
			go.AddOrGet<FloorTileExt>();
		}
	}

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
}
