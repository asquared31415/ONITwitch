using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitchCore;
using ONITwitchCore.Config;
using ONITwitchCore.Integration.DecorPackA;

namespace ONITwitch;

[UsedImplicitly]
public class OniTwitchMod : UserMod2
{
	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);
		LocString.CreateLocStringKeys(typeof(TIStrings.STRINGS), null);

		// load config
		var unusedConfig = MainConfig.Instance;
		var unusedCommandConfig = UserCommandConfigManager.Instance;
	}

	public const string DecorPackOneStaticID = "DecorPackA";

	public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
	{
		base.OnAllModsLoaded(harmony, mods);

		var decorPackEnabled = mods.Any(mod => (mod.staticID == DecorPackOneStaticID) && mod.IsEnabledForActiveDlc());
		if (decorPackEnabled)
		{
			DecorPack1Integration.LoadIntegration(harmony);
		}
	}
}
