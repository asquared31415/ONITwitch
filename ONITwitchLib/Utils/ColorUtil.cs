using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

/// <summary>
///     Methods and values related to common colors or operations.
/// </summary>
[PublicAPI]
public static class ColorUtil
{
	/// <summary>
	///     The color used to signal a warning.
	/// </summary>
	[PublicAPI] public static readonly Color RedWarningColor = new(0xFF / 256f, 0x30 / 256f, 0x30 / 256f);

	/// <summary>
	///     The color used to signal a success.
	/// </summary>
	[PublicAPI] public static readonly Color GreenSuccessColor = new(0x30 / 256f, 0xFF / 256f, 0x30 / 256f);

	/// <summary>
	///     The primary color used by Twitch for its logo.
	/// </summary>
	[PublicAPI] public static readonly Color PrimaryTwitchColor = new Color32(0x91, 0x46, 0xFF, 0xFF);

	/// <summary>
	///     A lighter version of the Twitch Purple, suitable for signaling "active" or "selected" things.
	/// </summary>
	[PublicAPI] public static readonly Color HighlightTwitchColor = new Color32(0xB5, 0x67, 0xFF, 0xFF);

	// most of the default twitch colors, some of the boring ones removed
	private static readonly Color[] Colors =
	{
		new(0.698f, 0.133f, 0.133f),
		new(1.000f, 0.500f, 0.314f),
		new(0.604f, 0.804f, 0.196f),
		new(1.000f, 0.271f, 0.000f),
		new(0.180f, 0.545f, 0.341f),
		new(0.855f, 0.647f, 0.125f),
		new(0.373f, 0.620f, 0.627f),
		new(0.118f, 0.565f, 1.000f),
		new(1.000f, 0.412f, 0.706f),
		new(0.541f, 0.169f, 0.886f),
		new(0.000f, 1.000f, 0.498f),
	};

	/// <summary>
	///     Parses a string with a hex color into a <see cref="Color" />.
	/// </summary>
	/// <param name="colorString">The string containing a color value in hex.</param>
	/// <param name="color">The output <see cref="Color" /> if the color was successfully parsed.</param>
	/// <returns><see langword="true" /> if the color was parsed, otherwise <see langword="false" /></returns>
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

	/// <returns>A random color that Twitch usernames can use by default.</returns>
	[PublicAPI]
	public static Color GetRandomTwitchColor()
	{
		return Colors.GetRandom();
	}
}
