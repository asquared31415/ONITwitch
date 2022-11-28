using KSerialization;
using ONITwitchCore.Patches;
using UnityEngine;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class PartyTime : KMonoBehaviour
{
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
