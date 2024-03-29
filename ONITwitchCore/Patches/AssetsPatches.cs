using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content;

namespace ONITwitch.Patches;

internal static class AssetsPatches
{
	[HarmonyPatch(typeof(Assets), "CreatePrefabs")]
	// ReSharper disable once InconsistentNaming
	private static class Assets_CreatePrefabs_Patch
	{
		[UsedImplicitly]
		private static void Postfix()
		{
			ComponentsExt.CollectFloorTiles();
		}
	}
}
