using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchLib;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class Vote
{
	private readonly List<VoteCount> votes = new();

	public Vote(List<EventInfo> choices)
	{
		foreach (var choice in choices)
		{
			votes.Add(new VoteCount(choice, 0));
		}
	}

	public void AddVote(int idx)
	{
		var choice = votes[idx];
		votes[idx] = choice with { Count = choice.Count + 1 };
	}

	[CanBeNull]
	public VoteCount? GetBestVote()
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

	public record struct VoteCount(EventInfo Info, int Count);
}
