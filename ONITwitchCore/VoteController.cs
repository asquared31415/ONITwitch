using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KSerialization;
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

	const float FIXMEVoteTime = 10;
	private const float FIXMEVoteDelay = 30;

	private TwitchChatConnection connection;

	public VotingState State { get; private set; } = VotingState.NotStarted;

	public float VoteTimeRemaining { get; private set; }
	public float VoteDelayRemaining { get; private set; }

	public Vote CurrentVote { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		connection = new TwitchChatConnection();
		connection.OnTwitchMessage += OnTwitchMessage;
		connection.Start();
	}

	public void StartVote()
	{
		Debug.Log("STARTING VOTE");

		var eventOptions = new List<EventInfo>();
		for (var idx = 0; idx < 3; idx++)
		{
			var entry = TwitchDeckManager.Instance.Draw();
			eventOptions.Add(entry);
		}

		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			Debug.Log($"{idx + 1}: {eventOptions[idx]}");
		}

		CurrentVote = new Vote(eventOptions);

		VoteTimeRemaining = FIXMEVoteTime;
		State = VotingState.VoteInProgress;
	}

	private void FinishVote()
	{
		var choice = CurrentVote.GetBestVote();
		Debug.Log("FINISHED VOTE");
		if (choice != null)
		{
			var data = DataManager.Instance.GetDataForEvent(choice.EventInfo);
			EventManager.Instance.TriggerEvent(choice.EventInfo, data);
		}
		else
		{
			// TODO: toast
			Debug.Log("No options were voted for");
		}

		CurrentVote = null;
		VoteDelayRemaining = FIXMEVoteDelay;
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
					VoteDelayRemaining -= Time.unscaledDeltaTime;
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
