using System.Collections.Generic;
using KSerialization;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimedGeyserTuning : KMonoBehaviour, ISim200ms
{
	[Serialize] private readonly List<TimedModification> modifications = new();

#pragma warning disable CS0649
	[MyCmpReq] private Geyser geyser;
#pragma warning restore CS0649

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (var timedModification in modifications)
		{
			geyser.AddModification(timedModification.Modification);
		}
	}

	public void AddModification(float time, Geyser.GeyserModification modification)
	{
		var timed = new TimedModification(time, modification);

		modifications.Add(timed);
		geyser.AddModification(modification);
	}

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
			}
		}
	}
}

[SerializationConfig(MemberSerialization.OptIn)]
public class TimedModification
{
	[Serialize] public float TimeRemaining;
	[Serialize] public Geyser.GeyserModification Modification;

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
