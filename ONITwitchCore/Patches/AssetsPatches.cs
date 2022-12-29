using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Content;

namespace ONITwitchCore.Patches;

public static class AssetsPatches
{
	[HarmonyPatch(typeof(Assets), "CreatePrefabs")]
	public static class Assets_CreatePrefabs_Patch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			ComponentsExt.CollectFloorTiles();
		}
	}
}
