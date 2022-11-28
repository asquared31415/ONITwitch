using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchCore;
using ONITwitchCore.Config;

namespace ONITwitch;

[UsedImplicitly]
public class OniTwitchMod : UserMod2
{
	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);

		// load config
		var unusedConfig = MainConfig.Instance;
		var unusedCommandConfig = UserCommandConfigManager.Instance;
	}
}
