using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Config;
using ONITwitch.Content.Cmps;
using ONITwitch.Settings.Components;
using ONITwitch.Toasts;
using ONITwitch.Voting;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class MainMenuPatches
{
	[HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
	// ReSharper disable once InconsistentNaming
	private static class MainMenu_OnPrefabInit_Patch
	{
		// ReSharper disable once InconsistentNaming
		[UsedImplicitly]
		private static void Postfix(MainMenu __instance)
		{
			var config = TwitchSettings.GetConfig();
			Log.Debug(
				$"Last opened settings v{config.LastOpenedSettingsVersion}, current v{TwitchSettings.CurrentConfigVersion}"
			);
			if (config.LastOpenedSettingsVersion < TwitchSettings.CurrentConfigVersion)
			{
				Log.Info(
					$"Last opened settings v{config.LastOpenedSettingsVersion}, current is v{TwitchSettings.CurrentConfigVersion}: showing notification"
				);
				var notification = __instance.gameObject.AddComponent<ModsButtonNotification>();
				notification.Message =
					STRINGS.ONITWITCH.UI.MAIN_MENU.NEW_SETTINGS.text.Colored(ColorUtil.HighlightTwitchColor);
			}
		}
	}


	[HarmonyPatch(typeof(MainMenu), "OnSpawn")]
	// ReSharper disable once InconsistentNaming
	private static class MainMenu_OnSpawn_Patch
	{
		[UsedImplicitly]
		private static void Postfix()
		{
			// This code sets up the vote controller
			// Since the vote controller is static, only run this once, before it's set up
			if (VoteController.Instance == null)
			{
				var (credentials, error) = CredentialsConfig.GetCredentials();
				if (error != null)
				{
					var toastGo = ToastManager.InstantiateToast(
						STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.TITLE,
						error
					);
					if ((toastGo != null) && toastGo.TryGetComponent(out OniTwitchToast toast))
					{
						toast.HoverTime = 60f;
					}
				}

				// Run this even if an error was set, because the credentials were set to an anonymous login
				var voteControllerGo = new GameObject("TwitchVoteController");
				voteControllerGo.AddComponent<VoteController>().Credentials = credentials;
				voteControllerGo.AddOrGet<VoteFile>();
				Object.DontDestroyOnLoad(voteControllerGo);
			}
		}
	}
}
