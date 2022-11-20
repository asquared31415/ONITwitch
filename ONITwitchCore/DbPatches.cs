using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchCore;

public static class DbPatches
{
	[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
	public static class Db_Initialize_Patch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			DefaultCommands.SetupCommands();
		}
	}
}
