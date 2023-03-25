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
		private static void Postfix()
		{
			ModAssets.LoadAssets();
			DefaultCommands.SetupCommands();
			CustomEffects.SetupEffects();
		}
	}
}
