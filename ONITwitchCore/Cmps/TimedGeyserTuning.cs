using KSerialization;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimedGeyserTuning : KMonoBehaviour, ISim200ms
{
	[Serialize] private float timeRemaining;
	[Serialize] private Geyser.GeyserModification geyserModification;

#pragma warning disable CS0649
	[MyCmpReq] private Geyser geyser;
#pragma warning restore CS0649

	public void Initialize(float time, Geyser.GeyserModification modification)
	{
		// remove the old modification if it exists
		if (geyser.modifications.Contains(geyserModification))
		{
			geyser.RemoveModification(geyserModification);
		}

		timeRemaining = time;
		geyserModification = modification;

		geyser.AddModification(geyserModification);
	}

	public void Sim200ms(float dt)
	{
		if (timeRemaining >= 0)
		{
			timeRemaining -= dt;
		}

		if (timeRemaining <= 0)
		{
			geyser.RemoveModification(geyserModification);

			enabled = false;
			Destroy(this);
		}
	}
}
