using System.Collections.Generic;

namespace ONITwitchLib;

public static class ListExt
{
	public static void ShuffleList<T>(this IList<T> list)
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
