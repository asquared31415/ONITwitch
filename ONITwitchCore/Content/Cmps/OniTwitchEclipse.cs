using KSerialization;
using UnityEngine;

namespace ONITwitch.Content.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class OniTwitchEclipse : KMonoBehaviour, ISim200ms
{
	public enum EclipseState
	{
		Normal,
		Eclipse,
	}

	[Serialize] [SerializeField] private float timeRemaining;

	public EclipseState State { get; private set; }

	public void Sim200ms(float dt)
	{
		if (State == EclipseState.Eclipse)
		{
			timeRemaining -= dt;
			if (timeRemaining <= 0)
			{
				State = EclipseState.Normal;
				timeRemaining = 0;
				if (TimeOfDay.Instance != null)
				{
					TimeOfDay.Instance.SetEclipse(false);
				}
			}
		}
	}

	public void StartEclipse(float time)
	{
		timeRemaining = time;
		State = EclipseState.Eclipse;
		if (TimeOfDay.Instance != null)
		{
			TimeOfDay.Instance.SetEclipse(true);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		State = timeRemaining > 0 ? EclipseState.Eclipse : EclipseState.Normal;
		if (TimeOfDay.Instance != null)
		{
			TimeOfDay.Instance.SetEclipse(State == EclipseState.Eclipse);
		}
	}
}
