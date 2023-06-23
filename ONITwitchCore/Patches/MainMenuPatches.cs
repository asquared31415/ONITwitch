using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Cmps;
using ONITwitch.Config;
using ONITwitch.Voting;
using UnityEngine;
using Object = UnityEngine.Object;
using ToastManager = ONITwitch.Toasts.ToastManager;

namespace ONITwitch.Patches;

internal static class MainMenuPatches
{
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
