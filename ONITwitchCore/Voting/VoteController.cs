using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using KSerialization;
using ONITwitch.Config;
using ONITwitch.Content.Cmps;
using ONITwitch.EventLib;
using ONITwitch.Settings.Components;
using ONITwitch.Toasts;
using ONITwitchLib.IRC;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;
using UnityEngine;

namespace ONITwitch.Voting;

[SerializationConfig(MemberSerialization.OptIn)]
internal class VoteController : KMonoBehaviour
{
	public enum VotingState
	{
		NotStarted,
		VoteInProgress,
		VoteDelay,
		Error,
	}

	public static VoteController Instance;

	// forgive any leading whitespace, then match at least 1 number
	private static readonly Regex StartsWithNumber = new(@"^\s*(?<num>\d+)");

	private TwitchConnection connection;

	internal Credentials Credentials;

	public VotingState State { get; private set; } = VotingState.NotStarted;

	public float VoteTimeRemaining { get; private set; }
	public float VoteDelayRemaining { get; private set; }

	[CanBeNull] public Vote CurrentVote { get; private set; }

	private void Update()
	{
		switch (State)
		{
			case VotingState.NotStarted:
			case VotingState.Error:
				break;
			case VotingState.VoteInProgress:
			{
				if (VoteTimeRemaining > 0)
				{
					// unscaled because it's real time
					VoteTimeRemaining -= Time.unscaledDeltaTime;
				}
				else
				{
					FinishVote();
				}

				break;
			}
			case VotingState.VoteDelay:
			{
				if (VoteDelayRemaining > 0)
				{
					var pauseShown = Traverse.Create(PauseScreen.Instance).Field<bool>("shown").Value;
					if (!pauseShown)
					{
						VoteDelayRemaining -= Time.unscaledDeltaTime;
					}
				}
				else
				{
					StartVote();
				}

				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();

		Instance = this;

		connection = new TwitchConnection();
		connection.OnReady += () =>
		{
			var configChannel = TwitchSettings.GetConfig().ChannelName;
			if (!string.IsNullOrEmpty(configChannel))
			{
				connection.JoinRoom(TwitchSettings.GetConfig().ChannelName);
			}
		};
		connection.OnChatMessage += OnTwitchMessage;
		connection.Start(Credentials);

		TwitchSettings.OnSettingsChanged += OnConfigChanged;
	}

	// returns whether the vote was successfully started
	public bool StartVote()
	{
		if (!connection.IsReady)
		{
			Log.Warn("Not yet authenticated, unable to start vote!");
			DialogUtil.MakeDialog(
				STRINGS.ONITWITCH.UI.DIALOGS.NOT_AUTHENTICATED.TITLE,
				STRINGS.ONITWITCH.UI.DIALOGS.NOT_AUTHENTICATED.BODY,
				UI.CONFIRMDIALOG.OK,
				null
			);
			return false;
		}

		var eventOptions = new List<EventInfo>();
		var drawnCount = 0;
		while (drawnCount < TwitchSettings.GetConfig().VoteCount)
		{
			var attempt = TwitchDeckManager.Instance.Draw();
			// if we fail to draw, exit early
			if (attempt == null)
			{
				break;
			}

			// Don't add duplicates
			if (!eventOptions.Contains(attempt))
			{
				eventOptions.Add(attempt);
				drawnCount += 1;
			}
		}

		if (eventOptions.Count == 0)
		{
			Log.Warn("Unable to draw any events! Canceling.");
			DialogUtil.MakeDialog(
				STRINGS.ONITWITCH.UI.DIALOGS.NO_VOTES.TITLE,
				STRINGS.ONITWITCH.UI.DIALOGS.NO_VOTES.BODY,
				UI.CONFIRMDIALOG.OK,
				null
			);
			State = VotingState.Error;
			return false;
		}

		var voteMsg = new StringBuilder(STRINGS.ONITWITCH.VOTE_CONTROLLER.START_VOTE_HEADER);
		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			voteMsg.Append($"{idx + 1}: {eventOptions[idx].FriendlyName} ");
		}

		Log.Info($"{voteMsg}");
		connection.SendTextMessage(TwitchSettings.GetConfig().ChannelName, voteMsg.ToString());

		CurrentVote = new Vote(eventOptions);

		if (TwitchSettings.GetConfig().ShowVoteStartToasts)
		{
			var toastMsg = new StringBuilder();
			for (var idx = 0; idx < eventOptions.Count; idx++)
			{
				toastMsg.Append($"{idx + 1}: {eventOptions[idx].FriendlyName}\n");
			}

			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.STARTING_VOTE.TITLE, toastMsg.ToString());
		}

		VoteTimeRemaining = TwitchSettings.GetConfig().VoteTime;
		State = VotingState.VoteInProgress;

		return true;
	}

	public void Stop()
	{
		State = VotingState.NotStarted;
		CurrentVote = null;
	}

	private void OnConfigChanged([NotNull] TwitchSettings.SettingsData config)
	{
		// TODO: leave old room
		connection.JoinRoom(config.ChannelName);
	}

	internal void SetError()
	{
		State = VotingState.Error;
	}

	private void FinishVote()
	{
		try
		{
			var choice = CurrentVote!.GetBestVote();
			string responseText;
			if (choice != null)
			{
				Log.Info($"Chosen vote was {choice.EventInfo}({choice.EventInfo.Id}) with {choice.Count} votes");
				var data = DataManager.Instance.GetDataForEvent(choice.EventInfo);
				choice.EventInfo.Trigger(data);
				responseText = string.Format(
					STRINGS.ONITWITCH.VOTE_CONTROLLER.CHOSEN_VOTE_FORMAT,
					choice.EventInfo,
					choice.Count
				);
			}
			else
			{
				ToastManager.InstantiateToast(
					STRINGS.ONITWITCH.TOASTS.END_VOTE_NO_OPTIONS.TITLE,
					STRINGS.ONITWITCH.TOASTS.END_VOTE_NO_OPTIONS.BODY
				);
				Log.Info("No options were voted for");
				responseText = STRINGS.ONITWITCH.VOTE_CONTROLLER.NO_VOTES;
			}

			connection.SendTextMessage(TwitchSettings.GetConfig().ChannelName, responseText);

			CurrentVote = null;
			VoteDelayRemaining = TwitchSettings.GetConfig().VoteDelay;
			State = VotingState.VoteDelay;
		}
		catch (Exception e)
		{
			State = VotingState.Error;

			Log.Warn($"Unhandled exception {e} while processing a voted event");
		}
	}

	// This may not be called on the main thread!!!
	private void OnTwitchMessage(TwitchMessage message)
	{
		MainThreadScheduler.Schedule(
			() =>
			{
				if ((State == VotingState.VoteInProgress) && (CurrentVote != null))
				{
					var match = StartsWithNumber.Match(message.Message);
					if (match.Success)
					{
						var numStr = match.Groups["num"].Value;
						if (int.TryParse(numStr, out var num))
						{
							CurrentVote.AddVote(message.UserInfo, num);
						}
					}
				}
			}
		);
	}
}
