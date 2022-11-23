using System;
using HarmonyLib;

namespace ONITwitchCore.Commands;

public class KillDupeCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count >= 2;
	}

	private static readonly Action<Health> HealthKillDelegate =
		AccessTools.MethodDelegate<Action<Health>>(AccessTools.DeclaredMethod(typeof(Health), "Kill"));

	public override void Run(object data)
	{
		var items = Components.LiveMinionIdentities.Items;
		if (items.Count > 0)
		{
			var rand = items.GetRandom();
			HealthKillDelegate(rand.gameObject.GetComponent<Health>());

			/*
			ToastUiManager.InstantiateToastWithGoTarget(
				"Killing Duplicant",
				$"{rand.name} has been killed :(",
				rand.gameObject
			);
			*/

			Debug.Log($"[Twitch Integration] Killed {rand.name}");
		}
		else
		{
			Debug.LogWarning(
				"[TwitchIntegration] Could not kill a random dupe (Do any live dupes exist?)"
			);
		}
	}
}
