using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;

namespace ONITwitchCore.Patches;

internal static class GamePatches
{
	[HarmonyPatch(typeof(Game), "OnSpawn")]
	private static class Game_OnSpawn_Patch
	{
		[UsedImplicitly]
		private static void Postfix(Game __instance)
		{
			__instance.gameObject.AddOrGet<VoteController>();
			__instance.gameObject.AddOrGet<VoteFile>();

			__instance.gameObject.AddOrGet<Eclipse>();
		}
	}
}
