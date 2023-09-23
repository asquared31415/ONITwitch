using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content;

namespace ONITwitch.Patches;

internal static class DbPatches
{
	[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
	// ReSharper disable once InconsistentNaming
	private static class Db_Initialize_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(Db __instance)
		{
			ModAssets.LoadAssets();
			DefaultCommands.SetupCommands();
			CustomEffects.SetupEffects();

			// initialize the extra db entries
			DbEx.Initialize(__instance.Root);
		}
	}
}
