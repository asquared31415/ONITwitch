using System.Collections.Generic;

namespace ONITwitchLib;

public static class DeckUtils
{
	public static List<T> RepeatList<T>(T item, int count)
	{
		var list = new List<T>(count);
		for (var idx = 0; idx < count; idx++)
		{
			list.Add(item);
		}

		return list;
	}
}
