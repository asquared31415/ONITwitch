using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KSerialization;
using ONITwitchCore.Config;
using ONITwitchLib;
using UnityEngine;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;
using EventManager = EventLib.EventManager;

namespace ONITwitchCore;

[SerializationConfig(MemberSerialization.OptIn)]
public class VoteController : KMonoBehaviour
{
	public enum VotingState
	{
		NotStarted,
		VoteInProgress,
		VoteDelay,
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
						connection.JoinRoom(MainConfig.Instance.ConfigData.Channel);
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
			Debug.LogWarning("[Twitch Integration] Not yet authenticated, unable to start vote!");
			return false;
		}

		var eventOptions = new List<EventInfo>();
		var drawnCount = 0;
		while (drawnCount < MainConfig.Instance.ConfigData.NumVotes)
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
			Debug.LogWarning("[Twitch Integration] Unable to draw any events! Canceling!");
			State = VotingState.NotStarted;
			return false;
		}

		var eventInst = EventManager.Instance;
		var voteMsg = new StringBuilder("Starting new vote! ");
		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			voteMsg.Append($"{idx + 1}: {eventInst.GetFriendlyName(eventOptions[idx])} ");
		}

		CurrentVote = new Vote(eventOptions);

		connection.SendTextMessage(MainConfig.Instance.ConfigData.Channel, voteMsg.ToString());

		VoteTimeRemaining = MainConfig.Instance.ConfigData.VoteTime;
		State = VotingState.VoteInProgress;

		return true;
	}

	private void FinishVote()
	{
		var choice = CurrentVote.GetBestVote();
		string responseText;
		if (choice != null)
		{
			var data = DataManager.Instance.GetDataForEvent(choice.EventInfo);
			EventManager.Instance.TriggerEvent(choice.EventInfo, data);
			responseText = $"The chosen vote was {choice.EventInfo} with {choice.Count} votes";
		}
		else
		{
			// TODO: toast
			Debug.Log("No options were voted for");
			responseText = "No options were voted for";
		}

		connection.SendTextMessage(MainConfig.Instance.ConfigData.Channel, responseText);

		CurrentVote = null;
		VoteDelayRemaining = MainConfig.Instance.ConfigData.CyclesPerVote * Constants.SECONDS_PER_CYCLE;
		State = VotingState.VoteDelay;
	}

	private void Update()
	{
		switch (State)
		{
			case VotingState.NotStarted:
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
					// explicitly using the *scaled* deltatime
					// the time remaining is a number of seconds for some count of cycles
					VoteDelayRemaining -= Time.deltaTime;
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
