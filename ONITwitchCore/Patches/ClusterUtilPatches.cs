using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content.Entities;

namespace ONITwitch.Patches;

internal static class ClusterUtilPatches
{
	// Make pocket dimensions considered to be a rocket interior for the build menu
	[HarmonyPatch(typeof(ClusterUtil), nameof(ClusterUtil.ActiveWorldIsRocketInterior))]
	// ReSharper disable once InconsistentNaming
	private static class ClusterUtil_ActiveWorldIsRocketInterior_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(ref bool __result)
		{
			if (!__result)
			{
				var world = ClusterManager.Instance.activeWorld;
				if (world != null)
				{
					if (world.gameObject.HasTag(PocketDimensionConfig.PocketDimensionEntityTag))
					{
						__result = true;
					}
				}
			}
		}
	}

	// Make pocket dimensions considered to have a telepad for the build menu
	[HarmonyPatch(typeof(ClusterUtil), nameof(ClusterUtil.ActiveWorldHasPrinter))]
	// ReSharper disable once InconsistentNaming
	private static class ClusterUtil_ActiveWorldHasPrinter_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(ref bool __result)
		{
			if (!__result)
			{
				var world = ClusterManager.Instance.activeWorld;
				if (world != null)
				{
					if (world.gameObject.HasTag(PocketDimensionConfig.PocketDimensionEntityTag))
					{
						__result = true;
					}
				}
			}
		}
	}
}
