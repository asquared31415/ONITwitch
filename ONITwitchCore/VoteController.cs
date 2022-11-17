using System.Collections;
using System.Collections.Generic;
using EventLib;
using UnityEngine;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class VoteController
{
	public enum VotingState
	{
		NotStarted,
		VoteInProgress,
		VoteDelay,
	}

	const float FIXMEVoteTime = 10;
	private const float FIXMEVoteDelay = 30;

	public VotingState State { get; private set; } = VotingState.NotStarted;

	public float VoteTimeRemaining { get; private set; }
	public float VoteDelayRemaining { get; private set; }

	public Vote CurrentVote { get; private set; }

	public VoteController()
	{
		// TODO set up connection for vote input
	}

	public void StartVote()
	{
		Debug.Log("STARTING VOTE");
		var eventInst = EventManager.Instance;
		var eventOptions = new List<EventInfo>
		{
			eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventA")!,
			eventInst.GetEventByID(DefaultCommands.CommandNamespace + "eventB")!,
		};

		CurrentVote = new Vote(eventOptions);

		// FIXME: DEBUG
		CurrentVote.AddVote(0);

		VoteTimeRemaining = FIXMEVoteTime;
		State = VotingState.VoteInProgress;

		Game.Instance.StartCoroutine(UpdateVoteTimer());
	}

	private IEnumerator UpdateVoteTimer()
	{
		while (VoteTimeRemaining > 0)
		{
			VoteTimeRemaining -= Time.unscaledDeltaTime;
			yield return null;
		}

		FinishVote();
	}

	private void FinishVote()
	{
		var choice = CurrentVote.GetBestVote();
		Debug.Log("FINISHED VOTE");
		Debug.Log(choice);
		if (choice.HasValue)
		{
			var (eventId, count) = choice.Value;
			var data = DataManager.Instance.GetDataForEvent(eventId);
			EventManager.Instance.TriggerEvent(eventId, data);
		}
		else
		{
			// TODO: toast
			Debug.Log("No options were voted for");
		}

		State = VotingState.VoteDelay;
		VoteDelayRemaining = FIXMEVoteDelay;
		Game.Instance.StartCoroutine(UpdateDelayTimer());
	}

	private IEnumerator UpdateDelayTimer()
	{
		while (VoteDelayRemaining > 0)
		{
			VoteDelayRemaining -= Time.unscaledDeltaTime;
			yield return null;
		}

		StartVote();
	}
}
