namespace ONITwitchLib.Utils;

public static class MathUtil
{
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
