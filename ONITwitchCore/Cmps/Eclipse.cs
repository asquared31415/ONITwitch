using KSerialization;
using UnityEngine;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class Eclipse : KMonoBehaviour, ISim200ms
{
	[Serialize] [SerializeField] private float timeRemaining;

	public EclipseState State { get; private set; }

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

	public enum EclipseState
	{
		Normal,
		Eclipse,
	}
}
