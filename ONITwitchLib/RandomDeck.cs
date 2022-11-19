using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ONITwitchLib;

public class RandomDeck<T>
{
	[NotNull] private readonly List<T> deck;
	// the index representing the top of the deck (the next entry to draw)
	private int headIdx;
	
	public RandomDeck([NotNull] List<T> deck)
	{
		this.deck = deck;
		this.deck.ShuffleList();
	}
	
	[NotNull]
	public T DrawEntry()
	{
		if (deck.Count == 0)
		{
			throw new Exception("Cannot draw from empty random deck");
		}
		
		var ret = deck[headIdx];
		headIdx += 1;
		if (headIdx >= deck.Count)
		{
			deck.ShuffleList();
			headIdx = 0;
		}

		return ret;
	}

	/// <summary>
	/// Adds an item to the deck and then re-shuffles the deck.
	/// For best results, prefer batching items to be added in an IEnumerable instead.
	/// </summary>
	/// <param name="item">The item to be added</param>
	public void AddAndShuffle([NotNull] T item)
	{
		deck.Add(item);
		deck.ShuffleList();
		headIdx = 0;
	}

	/// <summary>
	/// Adds a sequence of items to the deck and then re-shuffles the deck.
	/// </summary>
	/// <param name="items">A sequence of items to be added</param>
	public void AddAndShuffle([NotNull] IEnumerable<T> items)
	{
		deck.AddRange(items);
		deck.ShuffleList();
		headIdx = 0;
	}

	public void Shuffle()
	{
		deck.ShuffleList();
		headIdx = 0;
	}

	public void DebugPrintDeck()
	{
		foreach (var item in deck)
		{
			Debug.Log(item);
		}
	}
}
