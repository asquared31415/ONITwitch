using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitch.Settings.Components;
using ONITwitch.Toasts;
using ONITwitch.Voting;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.Commands;

internal class SpawnDupeCommand : CommandBase {
    private static readonly Dictionary<string, SpecialDupeData> SpecialDupes =
        new() { { "asquared31415", new SpecialDupeData("NAILS") } };

    public override bool Condition(object data) {
        var maxDupes = (double)((IDictionary<string, object>)data)["MaxDupes"];
        return Components.MinionIdentities.Count < maxDupes;
    }

    public override void Run(object data) {
        var config = TwitchSettings.GetConfig();

        ChosenVoter? chosenVoter = null;

        if (VoteController.Instance != null && VoteController.Instance.CurrentVote != null) {
            var votes = VoteController.Instance.CurrentVote.GetUserVotes().ToList();
            // Only get users that are not disallowed and are not spawned yet.
            var allowedVotes = votes.Where(
                    pair => {
                        if (config.DisallowedDupeNames.Any(
                                disallowed => string.Equals(
                                    pair.Key.DisplayName,
                                    disallowed,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )) {
                            return false;
                        }

                        return !Components.LiveMinionIdentities.Items.Any(
                            ([NotNull] i) => {
                                var normalizedName = i.name.ToLowerInvariant();
                                return normalizedName.Contains(pair.Key.DisplayName.ToLowerInvariant());
                            }
                        );
                    }
                )
                .ToList();

            if (allowedVotes.Count > 0) {
                (var user, var _) = allowedVotes.GetRandom();
                chosenVoter = new ChosenVoter(user.DisplayName, user.NameColor);
            }
        }

        var liveMinions = Components.LiveMinionIdentities.Items;
        if (liveMinions.Count == 0) {
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

        ApplyPersonality(minion, chosenVoter, config.UseTwitchNameColors);

        Vector3 pos;
        // First try to find a printing pod, since that should always be in a safe location.
        var pods = Components.Telepads.Items;
        if (pods.Count > 0) {
            pos = pods.GetRandom().transform.position;
        } else {
            Log.Debug("Unable to find any Telepads, using a random dupe's location instead");
            pos = liveMinions.GetRandom().transform.position;
        }

        minion.transform.SetLocalPosition(pos);

        var identity = minion.GetComponent<MinionIdentity>();
        identity.GetComponent<MinionResume>().ForceAddSkillPoint();
        var upgradeFx = new UpgradeFX.Instance(identity, new Vector3(0.0f, 0.0f, -0.1f));
        upgradeFx.StartSM();

        ToastManager.InstantiateToastWithGoTarget(
            STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.TITLE,
            string.Format(STRINGS.ONITWITCH.TOASTS.SPAWN_DUPE.BODY_FORMAT, identity.name),
            minion
        );

        Log.Info($"Spawned duplicant {identity.name}");
    }

    private static void ApplyPersonality(GameObject minion, ChosenVoter? voter, bool useTwitchColor) {
        if (voter is (var name, var color)) {
            if (SpecialDupes.TryGetValue(name, out var specialDupeData)) {
                var personalities = Db.Get().Personalities;

                var personality = personalities.TryGet(specialDupeData.PersonalityId);
                if (personality == null || !Game.IsDlcActiveForCurrentSave(personality.requiredDlcId)) {
                    personality = personalities.GetRandom(GameTags.Minions.Models.Standard, true, false);
                }

                new MinionStartingStats(personality).Apply(minion);
            } else {
                new MinionStartingStats(GameTags.Minions.Models.Standard, false).Apply(minion);
            }

            var finalColor = color ?? ColorUtil.GetRandomTwitchColor();
            minion.GetComponent<MinionIdentity>()
                .SetName(useTwitchColor ? $"<color=#{finalColor.ToHexString()}>{name}</color>" : name);
        } else {
            new MinionStartingStats(GameTags.Minions.Models.Standard, false).Apply(minion);
        }
    }

    private record struct ChosenVoter([NotNull] string Name, Color? Color);

    private record struct SpecialDupeData([NotNull] string PersonalityId);
}
