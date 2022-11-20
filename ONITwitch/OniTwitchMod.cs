using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchCore;

namespace ONITwitch;

[UsedImplicitly]
public class OniTwitchMod : UserMod2
{
	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);

		// load config
		var _ = MainConfig.Instance;
	}
}
