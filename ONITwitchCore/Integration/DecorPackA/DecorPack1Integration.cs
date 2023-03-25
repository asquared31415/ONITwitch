using System;
using HarmonyLib;
using ONITwitch.Integration.DecorPackA.Patches;
using ONITwitchLib.Logger;

namespace ONITwitch.Integration.DecorPackA;

internal static class DecorPack1Integration
{
	// loads integration with Decor Pack 1
	public static void LoadIntegration(Harmony harmony)
	{
		Log.Info("Loading integration with Decor Pack I");
		var moodLampConfigType = Type.GetType("DecorPackA.Buildings.MoodLamp.MoodLampConfig, DecorPackA");
		if (moodLampConfigType != null)
		{
			harmony.Patch(
				AccessTools.DeclaredMethod(moodLampConfigType, "DoPostConfigureComplete"),
				postfix: new HarmonyMethod(typeof(MoodLampConfig_DoPostConfigureComplete_Patch), "Postfix")
			);
		}
		else
		{
			Log.Warn("Unable to find MoodLampConfig from Decor Pack I");
		}
	}
}
