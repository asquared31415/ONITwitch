using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchCore;

public static class PauseMenuPatches
{
	private static readonly KButtonMenu.ButtonInfo TwitchButtonInfo = new(
		"Do the thing",
		Action.NumActions,
		OnTwitchButtonPressed
	);

	private static void OnTwitchButtonPressed()
	{
		Debug.Log("BUTTON PRESSED");
		TwitchButtonInfo.isEnabled = false;
		PauseScreen.Instance.RefreshButtons();

		new TwitchChatConnection().Start();
		/*
		var controller = new VoteController();
		controller.StartVote();
		*/
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
			buttons.Insert(4, TwitchButtonInfo);
			___buttons = buttons;
		}
	}
}
