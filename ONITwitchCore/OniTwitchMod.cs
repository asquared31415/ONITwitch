using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitch.EventLib;
using ONITwitch.Integration;
using ONITwitch.Integration.DecorPackA;
using UnityEngine;

namespace ONITwitch;

[UsedImplicitly]
internal class OniTwitchMod : UserMod2
{
	private const string DecorPackOneStaticID = "DecorPackA";
	internal static ModIntegration ModIntegration;

	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);

		var mainThreadObject = new GameObject("MainThreadScheduler");
		mainThreadObject.AddOrGet<MainThreadScheduler>();
		Object.DontDestroyOnLoad(mainThreadObject);

		RegisterDevTools();
	}

	public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
	{
		base.OnAllModsLoaded(harmony, mods);

		// register the static id reverse map for namespacing
		EventGroup.RegisterStaticIdMap(mods);

		ModIntegration = new ModIntegration(mods);

		if (ModIntegration.IsModPresentAndActive(DecorPackOneStaticID))
		{
			DecorPack1Integration.LoadIntegration(harmony);
		}
	}

	private static void RegisterDevTools()
	{
		var baseMethod = AccessTools.Method(typeof(DevToolManager), "RegisterDevTool");
		var twitchDevToolRegister = baseMethod.MakeGenericMethod(typeof(TwitchDevTool));
		twitchDevToolRegister.Invoke(DevToolManager.Instance, new object[] { "Mods/Twitch Integration" });
	}
}
