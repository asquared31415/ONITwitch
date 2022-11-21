using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
						connection.JoinRoom(MainConfig.Instance.Config.Channel);
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

		var condInst = ConditionsManager.Instance;
		var dataInst = DataManager.Instance;
		var dangerInst = DangerManager.Instance;

		var eventOptions = new List<EventInfo>();
		var attempts = 0;
		var drawnCount = 0;
		while (drawnCount < MainConfig.Instance.Config.NumVotes)
		{
			var entry = TwitchDeckManager.Instance.Draw();
			// Don't draw duplicates
			if (!eventOptions.Contains(entry))
			{
				// no danger assigned or danger within the expected range is okay
				var danger = dangerInst.GetDanger(entry);
				if ((danger == null) || ((MainConfig.Instance.Config.MinDanger <= danger.Value) &&
										 (danger.Value <= MainConfig.Instance.Config.MaxDanger)))
				{
					var data = dataInst.GetDataForEvent(entry);
					var condition = condInst.CheckCondition(entry, data);
					if (condition)
					{
						eventOptions.Add(entry);
						drawnCount += 1;
					}
				}
			}

			attempts += 1;
			if (attempts > MaxDrawAttempts)
			{
				Debug.LogWarning(
					$"[Twitch Integration] Reached maximum draw attempts of {MaxDrawAttempts} without drawing {MainConfig.Instance.Config.NumVotes} events!"
				);
				break;
			}
		}

		if (eventOptions.Count == 0)
		{
			Debug.LogWarning("[Twitch Integration] Unable to draw any events! Canceling!");
			State = VotingState.NotStarted;
			return false;
		}

		var voteMsg = new StringBuilder("Starting new vote! ");
		for (var idx = 0; idx < eventOptions.Count; idx++)
		{
			voteMsg.Append($"{idx + 1}: {eventOptions[idx]} ");
		}

		CurrentVote = new Vote(eventOptions);

		connection.SendTextMessage(MainConfig.Instance.Config.Channel, voteMsg.ToString());

		VoteTimeRemaining = MainConfig.Instance.Config.VoteTime;
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
		
		connection.SendTextMessage(MainConfig.Instance.Config.Channel, responseText);

		CurrentVote = null;
		VoteDelayRemaining = MainConfig.Instance.Config.CyclesPerVote * Constants.SECONDS_PER_CYCLE;
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
