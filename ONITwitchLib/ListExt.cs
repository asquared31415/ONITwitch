using System.Collections.Generic;
using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// Provides additional utilities for working with lists.
/// </summary>
[PublicAPI]
public static class ListExt
{
	/// <summary>
	/// Shuffles a list in-place using the Fisher-Yates algorithm.
	/// </summary>
	/// <param name="list">The list to shuffle.</param>
	/// <remarks>
	/// This may be called from any thread.
	/// This operation is O(n).
	/// </remarks>
	[PublicAPI]
	public static void ShuffleList<T>([NotNull] this IList<T> list)
	{
		// Fisher-Yates shuffle
		var n = list.Count;
		while (n > 1)
		{
			n--;
			var k = ThreadRandom.Next(n + 1);
			(list[k], list[n]) = (list[n], list[k]);
		}
	}
}
