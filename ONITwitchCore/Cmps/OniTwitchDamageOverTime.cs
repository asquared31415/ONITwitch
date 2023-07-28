using KSerialization;
using ONITwitch.Content;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitch.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class OniTwitchDamageOverTime : KMonoBehaviour
{
	[Serialize] private float accum;
	[Serialize] private float secondsPerTick;
	[Serialize] public float SecondsRemaining { get; private set; }

	private void Update()
	{
		if (health == null)
		{
			Log.Debug($"Poison damage over time missing health for {gameObject}");
			return;
		}

		if (SecondsRemaining > 0)
		{
			SecondsRemaining -= Time.deltaTime;
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
			UpdateStatusItem();
		}
	}

	public void StartPoison(float totalTime, int numTicks)
	{
		secondsPerTick = totalTime / numTicks;

		// remove one tick from the time remaining and accumulate it instantly to have the tick be instant
		SecondsRemaining = totalTime - secondsPerTick;
		accum = secondsPerTick;

		UpdateStatusItem();
	}

	private void UpdateStatusItem()
	{
		var statusItem = DbEx.ExtraStatusItems.PoisonedStatusItem;
		if (SecondsRemaining > 0)
		{
			// only add the status item if it doesn't already exist
			if (!selectable.HasStatusItem(statusItem))
			{
				selectable.AddStatusItem(statusItem, this);
			}
		}
		else
		{
			selectable.RemoveStatusItem(statusItem);
		}
	}

#pragma warning disable CS0649
	[MyCmpGet] private Health health;
	[MyCmpGet] private KSelectable selectable;
#pragma warning restore CS0649
}
