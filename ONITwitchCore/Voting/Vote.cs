using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchLib;
using ONITwitchLib.IRC;
using EventInfo = ONITwitch.EventLib.EventInfo;

namespace ONITwitch.Voting;

internal class Vote
{
	// map from user to index they voted for
	private readonly Dictionary<TwitchUserInfo, int> userVotes = new();
	private readonly List<VoteCount> votes = new();

	public Vote(List<EventInfo> choices)
	{
		if (choices.Count == 0)
		{
			throw new ArgumentException("there must be at least one vote choice", nameof(choices));
		}

		foreach (var choice in choices)
		{
			votes.Add(new VoteCount(choice, 0));
		}
	}

	internal ReadOnlyCollection<VoteCount> Votes => votes.AsReadOnly();

	public void AddVote(TwitchUserInfo user, int voteNum)
	{
		if ((voteNum <= 0) || (voteNum > votes.Count))
		{
			return;
		}

		// users are 1-based unfortunately
		var voteIdx = voteNum - 1;

		// move the user's vote if they voted already
		if (userVotes.TryGetValue(user, out var oldIdx))
		{
			votes[oldIdx].Count -= 1;
			votes[voteIdx].Count += 1;
		}
		else
		{
			votes[voteIdx].Count += 1;
		}

		userVotes[user] = voteIdx;
	}

	[CanBeNull]
	public VoteCount GetBestVote()
	{
		var maxVotes = votes.Max(vote => vote.Count);
		if (maxVotes == 0)
		{
			return null;
		}

		var tiedMaxVotes = votes.Where(vote => vote.Count == maxVotes).ToList();
		// count will always be at least 1 if we get here
		var randIdx = ThreadRandom.Next(tiedMaxVotes.Count);
		return tiedMaxVotes[randIdx];
	}

	[NotNull]
	public IReadOnlyDictionary<TwitchUserInfo, int> GetUserVotes()
	{
		return new ReadOnlyDictionary<TwitchUserInfo, int>(userVotes);
	}

	[NotNull]
	public List<TwitchUserInfo> GetAllVoters()
	{
		return userVotes.Keys.ToList();
	}

	internal class VoteCount(EventInfo eventInfo, int count)
	{
		public EventInfo EventInfo { get; } = eventInfo;
		public int Count { get; internal set; } = count;
	}
}
