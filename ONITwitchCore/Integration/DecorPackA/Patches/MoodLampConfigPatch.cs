using UnityEngine;

namespace ONITwitchCore.Integration.DecorPackA.Patches;

public static class MoodLampConfig_DoPostConfigureComplete_Patch
{
	public static void Postfix(GameObject go)
	{
		go.AddOrGet<GlitterMoodLampAccessor>();
	}
}
