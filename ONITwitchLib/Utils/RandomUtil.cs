using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

/// <summary>
/// Utilities for creating random values.
/// </summary>
[PublicAPI]
public static class RandomUtil
{
	/// <summary>
	/// Returns a random vector on a unit circle.
	/// </summary>
	/// <returns>A random vector on a unit circle.</returns>
	[PublicAPI]
	public static Vector2 OnUnitCircle()
	{
		var rand = Random.Range(0, 2 * Mathf.PI);
		return new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
	}

	/// <summary>
	/// Returns a vector that is on the unit circle and within an angle range.
	/// </summary>
	/// <remarks>Note that 0 degrees represents the vector (1, 0), and the angle increases counter-clockwise, such that 90 degrees is (0, 1).</remarks>
	/// <param name="minAngleInclusive">The lower bound of the angle of the output vector in degrees, inclusive.</param>
	/// <param name="maxAngleExclusive">The upper bound of the angle of the output vector in degrees, exclusive.</param>
	/// <returns>A vector with length one that has an angle between <paramref name="minAngleInclusive"/> and <paramref name="maxAngleExclusive"/>.</returns>
	[PublicAPI]
	public static Vector2 OnUnitCircleInRange(float minAngleInclusive, float maxAngleExclusive)
	{
		var rand = Mathf.Deg2Rad * Random.Range(minAngleInclusive, maxAngleExclusive);
		return new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
	}
}
