using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using ONITwitch.EventLib;
using ONITwitch.Integration;
using ONITwitch.Integration.DecorPackA;
using UnityEngine;
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

[HarmonyPatch(typeof(HoverTextConfiguration), "DrawTitle")]
internal static class CellNumInTitle
{
	[UsedImplicitly]
	// ReSharper disable once InconsistentNaming
	private static void Postfix(HoverTextDrawer drawer, HoverTextConfiguration __instance)
	{
		if (Camera.main != null)
		{
			var cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
			var pos = Grid.CellToPos(cell);
			drawer.NewLine();
			drawer.DrawText($"({pos.x}, {pos.y})", __instance.ToolTitleTextStyle);
			drawer.NewLine();
			drawer.DrawText($"Cell {cell}", __instance.ToolTitleTextStyle);
		}
	}
}

[HarmonyPatch(typeof(KImGuiUtil), nameof(KImGuiUtil.SetKAssertCB))]
// ReSharper disable once InconsistentNaming
internal static class ImGui_Patch
{
	[UsedImplicitly]
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
	{
		return new[] { new CodeInstruction(OpCodes.Ret) };
	}
}
