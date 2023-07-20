using System;
using System.Collections.Generic;
using System.Linq;
using ONITwitch.Settings;
using ONITwitch.Toasts;
using ONITwitch.Voting;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

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

		var config = GenericModSettings.GetConfig();

		if (VoteController.Instance != null)
		{
			var users = VoteController.Instance.SeenUsersById.Values.ToList();
			users.RemoveAll(
				info => config.DisallowedDupeNames.Any(
					disallowed => string.Equals(
						info.DisplayName,
						disallowed,
						StringComparison.InvariantCultureIgnoreCase
					)
				)
			);
			users.RemoveAll(
				n => Components.LiveMinionIdentities.Items.Any(
					i =>
					{
						var normalizedName = i.name.ToLowerInvariant();
						return normalizedName.Contains(n.DisplayName.ToLowerInvariant());
					}
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
			ToastManager.InstantiateToast(
				STRINGS.ONITWITCH.TOASTS.WARNINGS.EVENT_FAILURE,
				STRINGS.ONITWITCH.TOASTS.WARNINGS.SPAWN_DUPE_FAILURE.BODY
			);
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
			}
			else
			{
				new MinionStartingStats(false).Apply(minion);
			}

			var finalColor = color ?? ColorUtil.GetRandomTwitchColor();
			identity.SetName(
				config.UseTwitchNameColors
					? $"<color=#{finalColor.ToHexString()}>{name}</color>"
					: name
			);
		}
		else
		{
			new MinionStartingStats(false).Apply(minion);
		}

		identity.GetComponent<MinionResume>().ForceAddSkillPoint();

		Vector3 pos;
		// First try to find a printing pod, since that should always be in a safe location.
		var pods = Components.Telepads.Items;
		if (pods.Count > 0)
		{
			pos = pods.GetRandom().transform.position;
		}
		else
		{
			Log.Debug("Unable to find any Telepads, using a random dupe's location instead");
			pos = liveMinions.GetRandom().transform.position;
		}

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
