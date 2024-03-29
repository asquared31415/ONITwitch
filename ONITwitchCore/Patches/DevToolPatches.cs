using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.DevTools;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class DevToolPatches
{
	[HarmonyPatch(typeof(SelectTool), nameof(SelectTool.OnLeftClickDown))]
	// ReSharper disable once InconsistentNaming
	private static class SelectTool_OnLeftClickDown_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(SelectTool __instance)
		{
			if (TwitchDevTool.Instance != null)
			{
				TwitchDevTool.Instance.SelectedCell(__instance.GetSelectedCell());
			}
		}
	}

#if DEBUG
	[HarmonyPatch(typeof(DevToolManager), nameof(DevToolManager.UpdateShouldShowTools))]
	private static class DevToolKeybindFix
	{
		[UsedImplicitly]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		// ReSharper disable once RedundantAssignment by ref, is used
		private static void Postfix(ref bool ___showImGui)
		{
			if ((CameraController.Instance != null) && CameraController.Instance.FreeCameraEnabled)
			{
				___showImGui = false;
				return;
			}

			___showImGui = true;
		}
	}

	[HarmonyPatch(typeof(DevToolUI), nameof(DevToolUI.PingHoveredObject))]
	private static class DevToolNoPing
	{
		[UsedImplicitly]
		public static bool Prefix()
		{
			return false;
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
#endif
}
