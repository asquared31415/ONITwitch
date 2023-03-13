using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchCore.Integration.DecorPackA.Patches;

internal static class MoodLampConfig_DoPostConfigureComplete_Patch
{
	[UsedImplicitly]
	public static void Postfix(GameObject go)
	{
		go.AddOrGet<GlitterMoodLampAccessor>();
	}
}
