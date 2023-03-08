using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class PauseMenuPatches
{
	public static readonly KButtonMenu.ButtonInfo TwitchButtonInfo = new(
		STRINGS.UI.PAUSE_MENU.START_VOTES,
		Action.NumActions,
		OnTwitchButtonPressed
	);

	public static ColorStyleSetting TwitchButtonStyle;

	private static void OnTwitchButtonPressed()
	{
		TwitchButtonInfo.isEnabled = false;
		PauseScreen.Instance.RefreshButtons();

		var controller = Game.Instance.gameObject.AddOrGet<VoteController>();
		GameScheduler.Instance.ScheduleNextFrame(
			"ONITwitch.StartVotes",
			_ =>
			{
				var started = controller.StartVote();
				TwitchButtonInfo.isEnabled = !started;
				PauseScreen.Instance.RefreshButtons();
			}
		);
	}

	[HarmonyPatch(typeof(PauseScreen), "OnPrefabInit")]
	public static class PauseScreen_OnPrefabInit_Patch
	{
		[UsedImplicitly]
		// buttons is an array cast to IList in the PauseScreen
		// need to copy to a List and resize and reassign
		public static void Postfix(ref IList<KButtonMenu.ButtonInfo> ___buttons)
		{
			var buttons = ___buttons.ToList();
			TwitchButtonInfo.isEnabled = true;
			buttons.Insert(4, TwitchButtonInfo);
			___buttons = buttons;
		}
	}

	public static readonly Color DisabledColor = new Color32(0x6A, 0x69, 0x66, 0xFF);
	public static readonly Color InactiveTwitchColor = new Color32(0x91, 0x46, 0xFF, 0xFF);
	public static readonly Color HoverTwitchColor = new Color32(0xA2, 0x56, 0xFF, 0xFF);
	public static readonly Color PressedTwitchColor = new Color32(0xB5, 0x67, 0xFF, 0xFF);

	[HarmonyPatch(typeof(KButtonMenu), nameof(KButtonMenu.RefreshButtons))]
	public static class PauseScreen_RefreshButtons_Patch
	{
		[UsedImplicitly]
		public static void Postfix(KButtonMenu __instance)
		{
			if (__instance is PauseScreen && (TwitchButtonInfo.uibutton != null))
			{
				if ((TwitchButtonStyle == null) || (TwitchButtonInfo.uibutton.bgImage.colorStyleSetting == null) ||
					(TwitchButtonInfo.uibutton.bgImage.colorStyleSetting != TwitchButtonStyle))
				{
					TwitchButtonStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
					TwitchButtonStyle.disabledColor = DisabledColor;
					TwitchButtonStyle.inactiveColor = InactiveTwitchColor;
					TwitchButtonStyle.hoverColor = HoverTwitchColor;
					TwitchButtonStyle.activeColor = PressedTwitchColor;

					TwitchButtonInfo.uibutton.bgImage.colorStyleSetting = TwitchButtonStyle;
				}
			}
		}
	}
}
