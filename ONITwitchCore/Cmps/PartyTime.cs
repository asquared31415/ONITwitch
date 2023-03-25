using KSerialization;
using ONITwitch.Patches;
using UnityEngine;

namespace ONITwitch.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class PartyTime : KMonoBehaviour
{
	// ReSharper disable once InconsistentNaming
	[Serialize] [SerializeField] public float TimeRemaining;

	private void Update()
	{
		if (TimeRemaining > 0)
		{
			TimeRemaining -= Time.unscaledDeltaTime;
		}

		if (TimeRemaining <= 0)
		{
			PartyTimePatch.Enabled = false;
			enabled = false;
		}
	}
}
