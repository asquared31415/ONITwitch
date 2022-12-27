using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchLib;
using Random = UnityEngine.Random;

namespace ONITwitchCore;

public class VarietyShuffler<T>
{
	// collection of named groups
	private readonly Dictionary<string, Group> groups = new();

	[CanBeNull]
	public Group GetGroup([NotNull] string groupName)
	{
		return groups.TryGetValue(groupName, out var group) ? group : null;
	}

	public void AddItem([NotNull] T item, int weight)
	{
		// give every item added alone its own group
		var group = new Group((item, weight));
		// this is probably unique, and if it's not, that's an issue for whoever decided to copy this naming convention
		groups[GetItemDefaultGroupName(item)] = group;
	}

	// Throws when groupname already present
	public void AddGroup([NotNull] string groupName, [NotNull] Group group)
	{
		if (groups.ContainsKey(groupName))
		{
			throw new ArgumentException($"The group name `{groupName}` already exists", nameof(groupName));
		}

		groups.Add(groupName, group);
	}

	public void RemoveGroup([NotNull] string groupName)
	{
		if (!groups.Remove(groupName))
		{
			Debug.LogWarning($"[Twitch Integration] Unable to remove group {groupName}");
			Debug.Log(Environment.StackTrace);
		}
	}

	[MustUseReturnValue]
	[NotNull]
	public List<T> GetShuffled()
	{
		// based on https://engineering.atspotify.com/2014/02/how-to-shuffle-songs/
		var collectedOffsets = new List<(float, T)>();

		foreach (var (groupName, group) in groups)
		{
			if (group.TotalWeight <= 0)
			{
				Debug.LogWarning($"[Twitch Integration] Group {groupName} had invalid weight {group.TotalWeight}");
				continue;
			}

			// first spread the items out by separating them by 1/n and applying a random multiplier between 1-(1/n+1) and 1+(1/n+1)
			var nRecip = 1.0f / group.TotalWeight;
			var spaceMin = nRecip * (1 - 1.0f / (group.TotalWeight + 1));
			var spaceMax = nRecip * (1 + 1.0f / (group.TotalWeight + 1));

			var items = group.GetItems();
			// shuffle the items within a group before spreading them
			items.ShuffleList();

			// indexes correspond to the items above
			// the first item is always at offset 0
			var spaced = new List<float> { 0 };

			// TODO: this needs to run after every item except the last?

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

	internal string GetItemDefaultGroupName([NotNull] T item)
	{
		return $"__<nogroup>__{item.ToString()}_{item.GetHashCode()}_";
	}

	public class Group
	{
		private readonly Dictionary<T, int> weights = new();
		public int TotalWeight { get; private set; }

		public Group(params (T, int)[] entries)
		{
			foreach (var (item, weight) in entries)
			{
				weights[item] = weight;
				TotalWeight += weight;
			}
		}

		public void AddItem([NotNull] T item, int weight)
		{
			if (weights.ContainsKey(item))
			{
				weights[item] += weight;
			}
			else
			{
				weights[item] = weight;
			}

			TotalWeight += weight;
		}

		public void RemoveItem([NotNull] T item)
		{
			if (weights.ContainsKey(item))
			{
				TotalWeight -= weights[item];
				weights.Remove(item);
			}
		}

		[NotNull]
		internal List<T> GetItems()
		{
			var items = new List<T>();
			foreach (var (item, weight) in weights)
			{
				for (var i = 0; i < weight; i++)
				{
					items.Add(item);
				}
			}

			return items;
		}
	}
}
