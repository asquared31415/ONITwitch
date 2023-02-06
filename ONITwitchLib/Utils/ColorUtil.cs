using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

public static class ColorUtil
{
	public static Color RedWarningColor = new(0xFF / 256f, 0x30 / 256f, 0x30 / 256f);
	public static Color GreenSuccessColor = new(0x30 / 256f, 0xFF / 256f, 0x30 / 256f);

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
