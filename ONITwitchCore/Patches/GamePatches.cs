using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps;
using ONITwitch.Voting;

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
			__instance.gameObject.AddOrGet<OniTwitchEclipse>();
		}
	}

	[HarmonyPatch(typeof(Game), "DestroyInstances")]
	// ReSharper disable once InconsistentNaming
	public static class Game_OnCleanUp_Patch
	{
		[UsedImplicitly]
		private static void Postfix()
		{
			if (VoteController.Instance != null)
			{
				VoteController.Instance.Stop();
			}
		}
	}
}
