using UnityEngine;

namespace ONITwitchLib.Utils;

public static class StringExtensions
{
	public static string Colored(this string str, Color color)
	{
		return $"<color=#{color.ToHexString()}>{str}</color>";
	}
}
