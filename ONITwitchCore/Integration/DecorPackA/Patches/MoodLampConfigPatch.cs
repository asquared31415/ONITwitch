using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitch.Integration.DecorPackA.Patches;

// ReSharper disable once InconsistentNaming
internal static class MoodLampConfig_DoPostConfigureComplete_Patch
{
	[UsedImplicitly]
	public static void Postfix(GameObject go)
	{
		go.AddOrGet<OniTwitchGlitterMoodLampAccessor>();
	}
}
