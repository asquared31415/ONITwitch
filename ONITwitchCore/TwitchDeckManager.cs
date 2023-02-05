using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchCore.Config;
using ONITwitchCore.Settings;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventGroup = EventLib.EventGroup;
using EventInfo = EventLib.EventInfo;

namespace ONITwitchCore;

public class TwitchDeckManager
{
	private static TwitchDeckManager instance;

	[NotNull] public static TwitchDeckManager Instance => instance ??= new TwitchDeckManager();

	private readonly Dictionary<string, EventGroup> groups = new();

	private List<EventInfo> deck = new();
	private int headIdx;

	private TwitchDeckManager()
	{
	}

	public void AddGroup([NotNull] EventGroup group)
	{
		if (!groups.ContainsKey(group.Name))
		{
			group.OnGroupChanged += Shuffle;
			groups.Add(group.Name, group);
			Shuffle(group);
		}
	}

	[MustUseReturnValue]
	[CanBeNull]
	public EventGroup GetGroup([NotNull] string name)
	{
		return groups.TryGetValue(name, out var group) ? group : null;
	}

	[MustUseReturnValue]
	[NotNull]
	public IEnumerable<EventGroup> GetGroups()
	{
		return groups.Values;
	}

	[MustUseReturnValue]
	[CanBeNull]
	public EventInfo Draw()
	{
		const int maxDrawAttempts = 1000;

		var dataInst = DataManager.Instance;

		for (var attempts = 0; attempts < maxDrawAttempts; attempts++)
		{
			var entry = DrawEntry();
			// no danger assigned or danger within the expected range is okay
			var danger = entry.Danger;
			if ((danger == null) ||
				((GenericModSettings.Data.MinDanger <= danger.Value) &&
				 (danger.Value <= GenericModSettings.Data.MaxDanger)))
			{
				var data = dataInst.GetDataForEvent(entry);
				var condition = entry.CheckCondition(data);
				if (condition)
				{
					return entry;
				}
			}
		}

		Debug.LogWarning("[Twitch Integration] Unable to draw a command");
		return null;
	}

	private void Shuffle(EventGroup changedGroup)
	{
		// this design leaves room to possibly be smarter about shuffling in only the changed group

		deck = GetShuffled();
		headIdx = 0;
	}

	private EventInfo DrawEntry()
	{
		if (deck.Count == 0)
		{
			throw new Exception("Cannot draw from empty random deck");
		}

		var ret = deck[headIdx];
		headIdx += 1;
		if (headIdx >= deck.Count)
		{
			deck = GetShuffled();
			headIdx = 0;
		}

		return ret;
	}

	[MustUseReturnValue]
	[NotNull]
	private List<EventInfo> GetShuffled()
	{
		// based on https://engineering.atspotify.com/2014/02/how-to-shuffle-songs/
		var collectedOffsets = new List<(float, EventInfo)>();

		foreach (var (_, group) in groups)
		{
			if (group.TotalWeight < 0)
			{
				Debug.LogWarning($"[Twitch Integration] Group {group.Name} had invalid weight {group.TotalWeight}");
				continue;
			}

			if (group.TotalWeight == 0)
			{
				continue;
			}

			// first spread the items out by separating them by 1/n and applying a random multiplier between 1-(1/n+1) and 1+(1/n+1)
			var nRecip = 1.0f / group.TotalWeight;
			var spaceMin = nRecip * (1 - 1.0f / (group.TotalWeight + 1));
			var spaceMax = nRecip * (1 + 1.0f / (group.TotalWeight + 1));

			var items = new List<EventInfo>();
			foreach (var (item, weight) in group.GetWeights())
			{
				for (var i = 0; i < weight; i++)
				{
					items.Add(item);
				}
			}

			// shuffle the items within a group before spreading them
			items.ShuffleList();

			// indexes correspond to the items above
			// the first item is always at offset 0
			var spaced = new List<float> { 0 };

			// will never be larger than 1, since the max value approaches 1 as n approaches +inf
			// skip the first item, it was already added
			var endOffset = items.Skip(1)
				.Aggregate(
					0f,
					(accum, _) =>
					{
						var offset = UnityEngine.Random.Range(spaceMin, spaceMax);
						accum += offset;
						spaced.Add(accum);
						return accum;
					}
				);

			// will find a random value that would not bring the final value above offset 1
			var startOffset = UnityEngine.Random.Range(0, 1f - endOffset);

			// offset everything by the start offset and add it to the main collection
			collectedOffsets.AddRange(spaced.Select((t, idx) => (t + startOffset, items[idx])));
		}

		// sort by the offsets in ascending order
		collectedOffsets.Sort(
			(itemA, itemB) =>
			{
				var (offsetA, _) = itemA;
				var (offsetB, _) = itemB;

				return offsetA.CompareTo(offsetB);
			}
		);

		// return the items from the sorted list
		var ret = collectedOffsets.Select(itemOffset => itemOffset.Item2).ToList();
		return ret;
	}

	[Obsolete("Used as a cast helper for the reflection lib", true)]
	[MustUseReturnValue]
	[NotNull]
	[UsedImplicitly]
	private IEnumerable<object> InternalGetGroups()
	{
		return GetGroups();
	}
}
