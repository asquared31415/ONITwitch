using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;

// ReSharper disable InconsistentNaming

namespace ONITwitch.Patches;

internal static class TimelapserPatches
{
	[HarmonyPatch(typeof(Timelapser), "OnNewDay")]
	private static class Timelapser_OnNewDay_Patch
	{
		[UsedImplicitly]
		private static void Postfix([NotNull] List<int> ___worldsToScreenshot, ref bool ___screenshotToday)
		{
			// Remove all worlds that no longer exist or are pocket dimensions.
			___worldsToScreenshot.RemoveAll(
				static idx =>
				{
					var world = ClusterManager.Instance.GetWorld(idx);
					return (world == null) || world.HasTag(ExtraTags.PocketDimensionEntityTag);
				}
			);
			if (___worldsToScreenshot.Count == 0)
			{
				___screenshotToday = false;
			}
		}
	}
}
