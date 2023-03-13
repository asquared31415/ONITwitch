using KSerialization;
using ONITwitchCore.Patches;
using UnityEngine;

namespace ONITwitchCore.Cmps;

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
