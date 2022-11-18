using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchLib;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class Vote
{
	// map from user ID to index they voted for
	private readonly Dictionary<string, int> userVotes = new();
	private readonly List<VoteCount> votes = new();

	public Vote(List<EventInfo> choices)
	{
		foreach (var choice in choices)
		{
			votes.Add(new VoteCount(choice, 0));
		}
	}

	public void AddVote(string userId, int voteNum)
	{
		if ((voteNum <= 0) || (voteNum > votes.Count))
		{
			return;
		}

		// users are 1-based unfortunately
		var voteIdx = voteNum - 1;
		
		// move the user's vote if they voted already
		if (userVotes.TryGetValue(userId, out var oldIdx))
		{
			votes[oldIdx].Count -= 1;
			votes[voteIdx].Count += 1;
		}
		else
		{
			votes[voteIdx].Count += 1;
		}

		userVotes[userId] = voteIdx;
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

	public class VoteCount
	{
		public EventInfo EventInfo { get; internal set; }
		public int Count { get; internal set; }

		public VoteCount(EventInfo eventInfo, int count)
		{
			EventInfo = eventInfo;
			Count = count;
		}
	}
}
