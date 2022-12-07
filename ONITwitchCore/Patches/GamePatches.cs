using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;

namespace ONITwitchCore.Patches;

public static class GamePatches
{
	[HarmonyPatch(typeof(Game), "OnSpawn")]
	public static class Game_OnSpawn_Patch
	{
		[UsedImplicitly]
		public static void Postfix(Game __instance)
		{
			__instance.gameObject.AddOrGet<VoteController>();
			__instance.gameObject.AddOrGet<VoteFile>();

			__instance.gameObject.AddOrGet<Eclipse>();
		}
	}
}
