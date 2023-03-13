using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

/// <summary>
/// Methods and values related to common colors or operations.
/// </summary>
[PublicAPI]
public static class ColorUtil
{
	/// <summary>
	/// The color used to signal a warning.
	/// </summary>
	[PublicAPI] public static Color RedWarningColor = new(0xFF / 256f, 0x30 / 256f, 0x30 / 256f);

	/// <summary>
	/// The color used to signal a success.
	/// </summary>
	[PublicAPI] public static Color GreenSuccessColor = new(0x30 / 256f, 0xFF / 256f, 0x30 / 256f);

	/// <summary>
	/// Parses a string with a hex color into a <see cref="Color"/>.
	/// </summary>
	/// <param name="colorString">The string containing a color value in hex.</param>
	/// <param name="color">The output <see cref="Color"/> if the color was successfully parsed.</param>
	/// <returns><see langword="true"/> if the color was parsed, otherwise <see langword="false"/></returns>
	[PublicAPI]
	public static bool TryParseHexString([NotNull] string colorString, out Color color)
	{
		colorString = colorString.TrimStart('#');
		if (colorString.Length != 6)
		{
			color = Color.black;
			return false;
		}

		if (int.TryParse(
				colorString.Substring(0, 2),
				NumberStyles.HexNumber,
				CultureInfo.InvariantCulture,
				out var r
			) && int.TryParse(
				colorString.Substring(2, 2),
				NumberStyles.HexNumber,
				CultureInfo.InvariantCulture,
				out var g
			) && int.TryParse(
				colorString.Substring(4, 2),
				NumberStyles.HexNumber,
				CultureInfo.InvariantCulture,
				out var b
			))
		{
			color = new Color32((byte) r, (byte) g, (byte) b, byte.MaxValue);
			return true;
		}

		color = Color.black;
		return false;
	}
}
