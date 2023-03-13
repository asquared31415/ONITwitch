using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Content;

namespace ONITwitchCore.Patches;

internal static class DbPatches
{
	[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
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
