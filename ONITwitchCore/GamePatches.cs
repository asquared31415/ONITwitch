using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchCore;

public static class GamePatches
{
	[HarmonyPatch(typeof(Game), "OnSpawn")]
	public static class Game_OnSpawn_Patch
	{
		[UsedImplicitly]
		public static void Postfix(Game __instance)
		{
			__instance.gameObject.AddOrGet<VoteController>();
		}
	}
}
