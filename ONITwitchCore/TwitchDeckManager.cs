using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitch.Settings.Components;
using ONITwitchLib;
using ONITwitchLib.Logger;
using DataManager = ONITwitch.EventLib.DataManager;
using EventGroup = ONITwitch.EventLib.EventGroup;
using EventInfo = ONITwitch.EventLib.EventInfo;
using Random = UnityEngine.Random;

namespace ONITwitch;

/// <summary>
///     Provides methods to manipulate the deck of events
/// </summary>
[PublicAPI]
public class TwitchDeckManager
{
	private static TwitchDeckManager instance;

	private readonly Dictionary<string, EventGroup> groups = new();

	private List<EventInfo> deck = new();
	private int headIdx;

	private TwitchDeckManager()
	{
	}

	/// <summary>
	///     The instance of the deck manager.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public static TwitchDeckManager Instance => instance ??= new TwitchDeckManager();

	/// <summary>
	///     Adds an <see cref="EventGroup" /> of actions to the deck
	/// </summary>
	/// <param name="group"></param>
	[PublicAPI]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public void AddGroup([NotNull] EventGroup group)
	{
		if (groups.ContainsKey(group.Name))
		{
			return;
		}

		group.OnGroupChanged += Shuffle;
		groups.Add(group.Name, group);
		Shuffle(group);
	}

	/// <summary>
	///     Gets the <see cref="EventGroup" /> with the name specified by <paramref name="name" />, if it exists in the deck.
	/// </summary>
	/// <param name="name">The name of the group to retrieve.</param>
	/// <returns>The group, if it exists, otherwise <see langword="null" />.</returns>
	[PublicAPI]
	[MustUseReturnValue("This retrieves a group without modifying anything")]
	[CanBeNull]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public EventGroup GetGroup([NotNull] string name)
	{
		return groups.TryGetValue(name, out var group) ? group : null;
	}

	/// <summary>
	///     Gets all <see cref="EventGroup" />s registered in the deck.
	/// </summary>
	/// <returns>An enumerable containing the groups in the deck.</returns>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public IEnumerable<EventGroup> GetGroups()
	{
		return groups.Values;
	}

	/// <summary>
	///     Draws an <see cref="EventInfo" /> from the deck, shuffling if necessary.
	/// </summary>
	/// <returns>The drawn event.</returns>
	[PublicAPI]
	[MustUseReturnValue]
	[CanBeNull]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public EventInfo Draw()
	{
		const int maxDrawAttempts = 1000;

		var dataInst = DataManager.Instance;

		for (var attempts = 0; attempts < maxDrawAttempts; attempts++)
		{
			var entry = DrawEntry();
			// no danger assigned or danger within the expected range is okay
			var danger = entry.Danger;
			if (((danger == null) ||
				 ((TwitchSettings.GetConfig().MinDanger <= danger.Value) &&
				  (danger.Value <= TwitchSettings.GetConfig().MaxDanger))) &&
				entry.CheckCondition(dataInst.GetDataForEvent(entry)))
			{
				return entry;
			}
		}

		Log.Warn("Unable to draw a command");
		return null;
	}

	private void Shuffle(EventGroup changedGroup)
	{
		// this design leaves room to possibly be smarter about shuffling in only the changed group

		deck = GetShuffled();
		headIdx = 0;
	}

	[MustUseReturnValue]
	[NotNull]
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
			// ReSharper disable once ConvertIfStatementToSwitchStatement
			if (group.TotalWeight < 0)
			{
				Log.Warn($"Group {group.Name} had invalid weight {group.TotalWeight}");
				continue;
			}

			if (group.TotalWeight == 0)
			{
				continue;
			}

			// first spread the items out by separating them by 1/n and applying a random multiplier between 1-(1/n+1) and 1+(1/n+1)
			var reciprocal = 1.0f / group.TotalWeight;
			var spaceMin = reciprocal * (1 - 1.0f / (group.TotalWeight + 1));
			var spaceMax = reciprocal * (1 + 1.0f / (group.TotalWeight + 1));

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
						var offset = Random.Range(spaceMin, spaceMax);
						accum += offset;
						spaced.Add(accum);
						return accum;
					}
				);

			// will find a random value that would not bring the final value above offset 1
			var startOffset = Random.Range(0, 1f - endOffset);

			// offset everything by the start offset and add it to the main collection
			collectedOffsets.AddRange(spaced.Select((t, idx) => (t + startOffset, items[idx])));
		}

		// sort by the offsets in ascending order
		collectedOffsets.Sort(
			static (itemA, itemB) =>
			{
				var (offsetA, _) = itemA;
				var (offsetB, _) = itemB;

				return offsetA.CompareTo(offsetB);
			}
		);

		// return the items from the sorted list
		var ret = collectedOffsets.Select(static itemOffset => itemOffset.Item2).ToList();
		return ret;
	}

	[Obsolete("Used as a cast helper for the reflection lib", true)]
	[MustUseReturnValue]
	[NotNull]
	[PublicAPI(
		"This is not part of the public API. It exists solely for merged library internals. However, removing this is a breaking change."
	)]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	private IEnumerable<object> InternalGetGroups()
	{
		return GetGroups();
	}
}
