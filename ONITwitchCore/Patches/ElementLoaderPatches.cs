using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content.Elements;

namespace ONITwitch.Patches;

internal static class ElementLoaderPatches
{
	[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
	// ReSharper disable once InconsistentNaming
	private static class ElementLoader_Load_Patch
	{
		[UsedImplicitly]
		private static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
		{
			var defaultSubstances = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
			CustomElements.RegisterSubstances(defaultSubstances);
		}

		[UsedImplicitly]
		private static void Postfix()
		{
			CustomElements.FixTags();
		}
	}
}
