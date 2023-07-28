using System.Collections.Generic;
using KSerialization;
using ONITwitch.Content;

namespace ONITwitch.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class TimedGeyserTuning : KMonoBehaviour, ISim200ms
{
	[Serialize] private readonly List<TimedModification> modifications = new();

	public void Sim200ms(float dt)
	{
		// reverse loop to avoid needing to adjust indexes
		for (var idx = modifications.Count - 1; idx >= 0; idx--)
		{
			var timedModification = modifications[idx];
			timedModification.TimeRemaining -= dt;
			if (timedModification.TimeRemaining <= 0)
			{
				geyser.RemoveModification(timedModification.Modification);
				modifications.RemoveAt(idx);
				UpdateStatusItem();
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (var timedModification in modifications)
		{
			geyser.AddModification(timedModification.Modification);
		}

		UpdateStatusItem();
	}

	public void AddModification(float time, Geyser.GeyserModification modification)
	{
		var timed = new TimedModification(time, modification);

		modifications.Add(timed);
		geyser.AddModification(modification);

		UpdateStatusItem();
	}

	private void UpdateStatusItem()
	{
		if (selectable != null)
		{
			var statusItem = DbEx.ExtraStatusItems.GeyserTemporarilyTuned;
			if (modifications.Count > 0)
			{
				selectable.AddStatusItem(statusItem, modifications);
			}
			else
			{
				selectable.RemoveStatusItem(statusItem);
			}
		}
	}

#pragma warning disable CS0649
	[MyCmpReq] private Geyser geyser;
	[MyCmpGet] private KSelectable selectable;
#pragma warning restore CS0649
}

[SerializationConfig(MemberSerialization.OptIn)]
internal class TimedModification
{
	[Serialize] public Geyser.GeyserModification Modification;
	[Serialize] public float TimeRemaining;

	public TimedModification(float timeRemaining, Geyser.GeyserModification modification)
	{
		TimeRemaining = timeRemaining;
		Modification = modification;
	}

	public override string ToString()
	{
		return $"{Modification} - {TimeRemaining}s";
	}
}
