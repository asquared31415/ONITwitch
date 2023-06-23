using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitch.CustomElements;
using ONITwitch.Integration;
using ONITwitch.Integration.DecorPackA;
using ONITwitchLib;
using ONITwitchLib.Logger;
using TMPro;
using UnityEngine;
using EventGroup = ONITwitch.EventLib.EventGroup;
using Object = UnityEngine.Object;

namespace ONITwitch;

[UsedImplicitly]
internal class OniTwitchMod : UserMod2
{
	internal static ModIntegration ModIntegration;

	public override void OnLoad(Harmony harmony)
	{
		base.OnLoad(harmony);

		var mainThreadObject = new GameObject("MainThreadScheduler");
		mainThreadObject.AddOrGet<MainThreadScheduler>();
		Object.DontDestroyOnLoad(mainThreadObject);

		RegisterDevTools();
	}

	private const string DecorPackOneStaticID = "DecorPackA";

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
