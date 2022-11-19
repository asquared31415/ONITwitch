using System.Collections.Generic;
using EventLib;
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
			// TODO: properly generate options
			var eventInst = EventManager.Instance;
			var eventA = eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventA")!;
			var eventB = eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventB")!;

			TwitchDeckManager.Instance.AddToDeck(eventA);
			TwitchDeckManager.Instance.AddToDeck(eventB);

			var condInst = Conditions.Instance;
			condInst.AddCondition(
				eventA,
				data =>
				{
					Debug.Log("cond: does A contain \"uwu\"");
					if (data is List<string> eventData)
					{
						return eventData.Contains("uwu");
					}

					return false;
				}
			);
			condInst.AddCondition(
				eventA,
				_ =>
				{
					Debug.Log("Running cond A2 (pass)");
					return true;
				}
			);
		}
	}
}
