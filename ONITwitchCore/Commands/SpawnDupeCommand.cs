using System.Collections.Generic;
using System.Linq;
using ONITwitch.Settings;
using ONITwitch.Voting;
using ONITwitchLib.Logger;
using UnityEngine;
using ToastManager = ONITwitch.Toasts.ToastManager;

namespace ONITwitch.Commands;

internal class SpawnDupeCommand : CommandBase
{
	private static readonly Dictionary<string, SpecialDupeData> SpecialDupes = new()
	{
		{ "asquared31415", new SpecialDupeData("NAILS") },
	};

	public override bool Condition(object data)
	{
		var maxDupes = (double) ((IDictionary<string, object>) data)["MaxDupes"];
		return Components.MinionIdentities.Count < maxDupes;
	}

	public override void Run(object data)
	{
		string name = null;
		Color? color = null;
		if (VoteController.Instance != null)
		{
			var users = VoteController.Instance.SeenUsersById.Values.ToList();
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

		var identity = minion.GetComponent<MinionIdentity>();
		if (name != null)
		{
			if (SpecialDupes.TryGetValue(name, out var specialDupeData))
			{
				var personalities = Db.Get().Personalities;
				var personality = personalities.TryGet(specialDupeData.PersonalityId) ??
								  personalities.GetRandom(true, false);
				new MinionStartingStats(personality).Apply(minion);

				identity.SetName(
					GenericModSettings.Data.UseTwitchNameColors && color.HasValue
						? $"<color=#{color.Value.ToHexString()}>{name}</color>"
						: name
				);
			}
			else
			{
				new MinionStartingStats(false).Apply(minion);
				identity.SetName(
					GenericModSettings.Data.UseTwitchNameColors && color.HasValue
						? $"<color=#{color.Value.ToHexString()}>{name}</color>"
						: name
				);
			}
		}
		else
		{
			new MinionStartingStats(false).Apply(minion);
		}

		identity.GetComponent<MinionResume>().ForceAddSkillPoint();

		var pos = Components.LiveMinionIdentities.Items.GetRandom().transform.position;
		minion.transform.SetLocalPosition(pos);

		var upgradeFx = new UpgradeFX.Instance(identity, new Vector3(0.0f, 0.0f, -0.1f));
		upgradeFx.StartSM();

		ToastManager.InstantiateToastWithGoTarget(
			STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.TITLE,
			string.Format(STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.BODY_FORMAT, identity.name),
			minion
		);

		Log.Info($"Spawned duplicant {identity.name}");
	}

	internal class SpecialDupeData
	{
		internal string PersonalityId;

		internal SpecialDupeData(string personalityId)
		{
			PersonalityId = personalityId;
		}
	}
}
