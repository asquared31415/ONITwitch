using System;
using HarmonyLib;
using ONITwitchCore.Integration.DecorPackA.Patches;

namespace ONITwitchCore.Integration.DecorPackA;

public static class DecorPack1Integration
{
	// loads integration with Decor Pack 1
	public static void LoadIntegration(Harmony harmony)
	{
		Debug.Log("[Twitch Integration] Loading integration with Decor Pack I");
		var moodLampConfigType = Type.GetType("DecorPackA.Buildings.MoodLamp.MoodLampConfig, DecorPackA");
		if (moodLampConfigType == null)
		{
			Debug.LogWarning("[Twitch Integration] Unable to find MoodLampConfig from Decor Pack I");
		}

		harmony.Patch(
			AccessTools.DeclaredMethod(moodLampConfigType, "DoPostConfigureComplete"),
			postfix: new HarmonyMethod(typeof(MoodLampConfig_DoPostConfigureComplete_Patch), "Postfix")
		);

		Debug.Log("[Twitch Integration] Decor Pack I integration complete!");
	}
}
