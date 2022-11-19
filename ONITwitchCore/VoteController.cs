using System;
using System.Collections.Generic;
using System.Text;
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

	// The maximum number of times to attempt to draw
	private const int MaxDrawAttempts = 100;

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

		var condInst = Conditions.Instance;
		var dataInst = DataManager.Instance;

		var eventOptions = new List<EventInfo>();
		const int TODODrawCount = 3;
		var attempts = 0;
		var drawnCount = 0;
		while (drawnCount < TODODrawCount)
		{
			var entry = TwitchDeckManager.Instance.Draw();
			var data = dataInst.GetDataForEvent(entry);
			var condition = condInst.CheckCondition(entry, data);
			if (condition)
			{
				eventOptions.Add(entry);
				drawnCount += 1;
			}

			attempts += 1;
			if (attempts > MaxDrawAttempts)
			{
				Debug.LogWarning(
					$"[Twitch Integration] Reached maximum draw attempts of {MaxDrawAttempts} without drawing {TODODrawCount} events!"
				);
				break;
			}
		}

		if (eventOptions.Count == 0)
		{
			Debug.LogWarning("[Twitch Integration] Unable to draw any events! Canceling!");
			State = VotingState.NotStarted;
			return;
		}

		var voteMsg = new StringBuilder("Starting new vote! ");
		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			voteMsg.Append($"{idx + 1}: {eventOptions[idx]} ");
		}

		CurrentVote = new Vote(eventOptions);

		connection.SendTextMessage("asquared31415", voteMsg.ToString());

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
