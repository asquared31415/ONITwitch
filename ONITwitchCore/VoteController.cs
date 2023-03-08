using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using KSerialization;
using ONITwitchCore.Settings;
using ONITwitchLib;
using ONITwitchLib.Logger;
using UnityEngine;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore;

[SerializationConfig(MemberSerialization.OptIn)]
public class VoteController : KMonoBehaviour
{
	public enum VotingState
	{
		NotStarted,
		VoteInProgress,
		VoteDelay,
		Error
	}

	private TwitchChatConnection connection;

	public VotingState State { get; private set; } = VotingState.NotStarted;

	public float VoteTimeRemaining { get; private set; }
	public float VoteDelayRemaining { get; private set; }

	public Vote CurrentVote { get; private set; }

	public readonly Dictionary<string, TwitchUserInfo> SeenUsersById = new();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		connection = new TwitchChatConnection();
		connection.OnTwitchMessage += OnTwitchMessage;
		connection.Start();
		Task.Run(
			() =>
			{
				while (true)
				{
					if (connection.IsAuthenticated)
					{
						connection.JoinRoom(GenericModSettings.Data.ChannelName);
						break;
					}

					Thread.Sleep(200);
				}
			}
		);
	}

	// returns whether the vote was successfully started
	public bool StartVote()
	{
		if (!connection.IsAuthenticated)
		{
			Log.Warn("Not yet authenticated, unable to start vote!");
			return false;
		}

		var eventOptions = new List<EventInfo>();
		var drawnCount = 0;
		while (drawnCount < GenericModSettings.Data.VoteCount)
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
			State = VotingState.Error;
			return false;
		}

		var voteMsg = new StringBuilder("Starting new vote! ");
		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			voteMsg.Append($"{idx + 1}: {eventOptions[idx].FriendlyName} ");
		}

		Log.Info($"{voteMsg}");

		CurrentVote = new Vote(eventOptions);

		connection.SendTextMessage(GenericModSettings.Data.ChannelName, voteMsg.ToString());

		if (GenericModSettings.Data.ShowVoteStartToasts)
		{
			var toastMsg = new StringBuilder();
			for (var idx = 0; idx < eventOptions.Count; idx++)
			{
				toastMsg.Append($"{idx + 1}: {eventOptions[idx].FriendlyName}\n");
			}

			ToastManager.InstantiateToast(STRINGS.TOASTS.STARTING_VOTE.TITLE, toastMsg.ToString());
		}

		VoteTimeRemaining = GenericModSettings.Data.VoteTime;
		State = VotingState.VoteInProgress;

		return true;
	}

	internal void SetError()
	{
		State = VotingState.Error;
	}

	private void FinishVote()
	{
		try
		{
			var choice = CurrentVote.GetBestVote();
			string responseText;
			if (choice != null)
			{
				Log.Info($"Chosen vote was {choice.EventInfo}({choice.EventInfo.Id}) with {choice.Count} votes");
				var data = DataManager.Instance.GetDataForEvent(choice.EventInfo);
				choice.EventInfo.Trigger(data);
				responseText = $"The chosen vote was {choice.EventInfo} with {choice.Count} votes";
			}
			else
			{
				ToastManager.InstantiateToast(
					STRINGS.TOASTS.END_VOTE_NO_OPTIONS.TITLE,
					STRINGS.TOASTS.END_VOTE_NO_OPTIONS.BODY
				);
				Log.Info("No options were voted for");
				responseText = "No options were voted for";
			}

			connection.SendTextMessage(GenericModSettings.Data.ChannelName, responseText);

			CurrentVote = null;
			VoteDelayRemaining = GenericModSettings.Data.VoteDelay;
			State = VotingState.VoteDelay;
		}
		catch (Exception e)
		{
			State = VotingState.Error;

			Log.Warn($"Unhandled exception {e} while processing a voted event");
		}
	}

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

	protected override void OnCleanUp()
	{
		State = VotingState.NotStarted;
		CurrentVote = null;
		base.OnCleanUp();
	}

	// forgive any leading whitespace, then match at least 1 number
	private static readonly Regex StartsWithNumber = new(@"^\s*(?<num>\d+)");

	private void OnTwitchMessage(TwitchMessage message)
	{
		SeenUsersById[message.UserInfo.UserId] = message.UserInfo;
		if ((State == VotingState.VoteInProgress) && (CurrentVote != null))
		{
			var userId = message.UserInfo.UserId;
			var match = StartsWithNumber.Match(message.Message);
			if (match.Success)
			{
				var numStr = match.Groups["num"].Value;
				if (int.TryParse(numStr, out var num))
				{
					CurrentVote.AddVote(userId, num);
				}
			}
		}
	}
}
