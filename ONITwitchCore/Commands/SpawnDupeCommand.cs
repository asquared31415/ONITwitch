using System.Collections.Generic;
using System.Linq;
using ONITwitchCore.Config;
using ONITwitchCore.Settings;
using ONITwitchCore.Toasts;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitchCore.Commands;

internal class SpawnDupeCommand : CommandBase
{
	public override bool Condition(object data)
	{
		var maxDupes = (double) ((IDictionary<string, object>) data)["MaxDupes"];
		return Components.MinionIdentities.Count < maxDupes;
	}

	public override void Run(object data)
	{
		string name = null;
		Color? color = null;
		if ((Game.Instance != null) &&
			Game.Instance.TryGetComponent<VoteController>(out var voteController))
		{
			var users = voteController.SeenUsersById.Values.ToList();
			users.RemoveAll(
				n => Components.LiveMinionIdentities.Items.Any(
					i => i.name.ToLowerInvariant().Contains(n.DisplayName.ToLowerInvariant())
				)
			);

			if (users.Count > 0)
			{
				var user = users.GetRandom();
				name = user.DisplayName;
				color = user.NameColor;
			}
		}

		var liveMinions = Components.LiveMinionIdentities.Items;
		if (liveMinions.Count == 0)
		{
			Log.Warn("No live minions, aborting spawn");
			return;
		}

		var minion = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		minion.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(minion);
		minion.SetActive(true);
		new MinionStartingStats(false).Apply(minion);

		var identity = minion.GetComponent<MinionIdentity>();
		if (name != null)
		{
			identity.SetName(
				GenericModSettings.Data.UseTwitchNameColors && color.HasValue
					? $"<color=#{color.Value.ToHexString()}>{name}</color>"
					: name
			);
		}

		var resume = identity.GetComponent<MinionResume>();
		if (resume != null)
		{
			resume.ForceAddSkillPoint();
		}

		var pos = Components.LiveMinionIdentities.Items.GetRandom();
		minion.transform.SetLocalPosition(pos.transform.position);

		ToastManager.InstantiateToastWithGoTarget(
			STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.TITLE,
			string.Format(STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.BODY_FORMAT, identity.name),
			minion
		);

		Log.Info($"Spawned duplicant {identity.name}");
	}
}
