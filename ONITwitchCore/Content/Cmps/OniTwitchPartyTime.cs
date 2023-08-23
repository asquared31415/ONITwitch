using KSerialization;
using ONITwitch.Patches;
using UnityEngine;

namespace ONITwitch.Content.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class OniTwitchPartyTime : KMonoBehaviour
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
