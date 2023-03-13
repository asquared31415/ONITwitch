using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

/// <summary>
/// Extensions to strings
/// </summary>
[PublicAPI]
public static class StringExtensions
{
	/// <summary>
	/// Creates a string that has a color when displayed by TextMesh Pro.
	/// </summary>
	/// <param name="str">The string to color.</param>
	/// <param name="color">The color of the string.</param>
	/// <returns>A new string that has the specified color.</returns>
	[PublicAPI]
	public static string Colored(this string str, Color color)
	{
		return $"<color=#{color.ToHexString()}>{str}</color>";
	}
}
