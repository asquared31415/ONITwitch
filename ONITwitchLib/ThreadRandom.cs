using System;

namespace ONITwitchLib;

public class ThreadRandom
{
	private static readonly object LockObj = new();
	private static readonly Random Random = new();

	public static int Next()
	{
		lock (LockObj)
		{
			return Random.Next();
		}
	}

	public static int Next(int exclusiveMax)
	{
		lock (LockObj)
		{
			return Random.Next(exclusiveMax);
		}
	}

	public static int Next(int inclusiveMin, int exclusiveMax)
	{
		lock (LockObj)
		{
			return Random.Next(inclusiveMin, exclusiveMax);
		}
	}

	public static void NextBytes(byte[] buf)
	{
		lock (LockObj)
		{
			Random.NextBytes(buf);
		}
	}

	public static double NextDouble()
	{
		lock (LockObj)
		{
			return Random.NextDouble();
		}
	}
}
