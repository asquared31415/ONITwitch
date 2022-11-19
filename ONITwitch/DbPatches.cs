using EventLib;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore;

namespace ONITwitch;

public static class DbPatches
{
	[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
	public static class Db_Initialize_Patch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			// TODO: properly generate options
			var eventInst = EventManager.Instance;
			var eventA = eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventA")!;
			var eventB = eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventB")!;
		
			TwitchDeckManager.Instance.AddToDeck(eventA);
			TwitchDeckManager.Instance.AddToDeck(eventB);
		}
	}
}
