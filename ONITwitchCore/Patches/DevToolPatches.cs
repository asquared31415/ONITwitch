using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Logger;
using UnityEngine.EventSystems;

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
#endif
}
