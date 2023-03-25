using KSerialization;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitch.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class DamageOverTime : KMonoBehaviour
{
	[Serialize] private float secondsRemaining;
	[Serialize] private float secondsPerTick;
	[Serialize] private float accum;

#pragma warning disable CS0649
	[MyCmpGet] private Health health;
#pragma warning restore CS0649

	public void StartPoison(float totalTime, int numTicks)
	{
		secondsPerTick = totalTime / numTicks;

		// remove one tick from the time remaining and accumulate it instantly to have the tick be instant
		secondsRemaining = totalTime - secondsPerTick;
		accum = secondsPerTick;
	}

	private void Update()
	{
		if (health == null)
		{
			Log.Debug($"Poison damage over time missing health for {gameObject}");
			return;
		}

		if (secondsRemaining > 0)
		{
			secondsRemaining -= Time.deltaTime;
			accum += Time.deltaTime;
			while (accum >= secondsPerTick)
			{
				accum -= secondsPerTick;
				health.Damage(10f);
			}
		}
		else
		{
			enabled = false;
		}
	}
}
