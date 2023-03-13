using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

/// <summary>
/// Utilities for math things.
/// </summary>
[PublicAPI]
public static class MathUtil
{
	/// <summary>
	/// Gets the distance between two numbers in range [0,1), if numbers wrapped at 1.0.
	/// </summary>
	/// <param name="a">The first number.</param>
	/// <param name="b">The second number.</param>
	/// <returns>The smallest distance between the two numbers.</returns>
	/// <remarks>This can be useful to determine the difference in phase between two periodic functions.</remarks>
	[PublicAPI]
	public static float ShortestDistanceModuloOne(float a, float b)
	{
		// Ensure b > a without changing distance mod 1
		if (b < a)
		{
			b += 1;
		}

		var diff = b - a;
		// if the difference is greater than a half circle, the shortest way is the other way around
		if (diff > 0.5)
		{
			return -(1.0f - diff);
		}

		return diff;
	}
}
