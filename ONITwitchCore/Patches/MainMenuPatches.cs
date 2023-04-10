using System;
using System.Net;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Cmps;
using ONITwitch.Config;
using ONITwitch.Voting;
using ONITwitchLib.Logger;
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
			var credentials = CredentialsConfig.Instance.Credentials;
			ValidateNick(credentials.Nick);
			ValidateToken(credentials.Oauth);

			var voteControllerGo = new GameObject("TwitchVoteController");
			voteControllerGo.AddComponent<VoteController>();
			Object.DontDestroyOnLoad(voteControllerGo);
		}

		private static readonly Regex NickRegex = new("^[A-Za-z0-9][A-Za-z0-9_]*$");

		private static void ValidateNick(string nick)
		{
			string errMsg = null;
			if (nick.IsNullOrWhiteSpace())
			{
				Log.Warn("Null or whitespace nick in credentials");
				errMsg = STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.NO_LOGIN;
			}
			else if (nick.Contains("/") || nick.Contains("\\"))
			{
				Log.Warn($"Nick contained a slash: {nick}");
				errMsg = STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.NICK_CONTAINS_SLASH;
			}
			else if (!NickRegex.IsMatch(nick))
			{
				Log.Warn($"Nick did not match ^[A-Za-z0-9][A-Za-z0-9_]*$ {nick}");
				errMsg = STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.INVALID_NICK;
			}

			if (!errMsg.IsNullOrWhiteSpace())
			{
				var toastGo = ToastManager.InstantiateToast(
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.TITLE,
					errMsg
				);
				if ((toastGo != null) && toastGo.TryGetComponent(out OniTwitchToast toast))
				{
					toast.HoverTime = 60f;
				}
			}
		}

		private static readonly Regex OauthRegex = new("[0-9a-zA-Z]+");

		private static void ValidateToken(string oauth)
		{
			// strip the oauth: prefix for all of the processing
			if (oauth.IndexOf("oauth:", StringComparison.Ordinal) != -1)
			{
				oauth = oauth.Substring("oauth:".Length);
			}

			string errMsg = null;
			var match = OauthRegex.Match(oauth);
			if (!match.Success || (match.Index != 0))
			{
				Log.Warn("oauth did not match the regex `[0-9a-zA-Z]+`");
				errMsg = STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.MALFORMED_OAUTH;
			}
			else
			{
				// validate the token using the twitch endpoint
				const string twitchTokenValidateUri = "https://id.twitch.tv/oauth2/validate";

				var request = WebRequest.CreateHttp(twitchTokenValidateUri);
				var headers = new WebHeaderCollection { { "Authorization", $"Bearer {oauth}" } };
				request.Headers = headers;
				try
				{
					// if this succeeds, the error message will not be set
					request.GetResponse();
				}
				catch (WebException we)
				{
					using var r = we.Response;
					if (r != null)
					{
						var httpResponse = (HttpWebResponse) r;
						Log.Warn($"Error validating oauth token with twitch.  Status: {httpResponse.StatusCode}");
						errMsg = httpResponse.StatusCode == HttpStatusCode.Unauthorized
							? STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.EXPIRED_OAUTH
							: STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.UNKNOWN_OAUTH_ERR;
					}
					else
					{
						Log.Warn("Error validating oauth token with twitch.  No response.");
						errMsg = STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.CONNECTION_OAUTH_ERR;
					}
				}
			}

			if (!errMsg.IsNullOrWhiteSpace())
			{
				var toastGo = ToastManager.InstantiateToast(
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.TITLE,
					errMsg
				);
				if ((toastGo != null) && toastGo.TryGetComponent(out OniTwitchToast toast))
				{
					toast.HoverTime = 60f;
				}
			}
		}
	}
}
