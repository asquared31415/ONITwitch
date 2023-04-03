using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Cmps;

namespace ONITwitch.Patches;

internal static class GamePatches
{
	[HarmonyPatch(typeof(Game), "OnSpawn")]
	// ReSharper disable once InconsistentNaming
	private static class Game_OnSpawn_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(Game __instance)
		{
			__instance.gameObject.AddOrGet<VoteController>();
			__instance.gameObject.AddOrGet<VoteFile>();

			__instance.gameObject.AddOrGet<OniTwitchEclipse>();
		}
	}
}
