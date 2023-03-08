using System;
using System.Net;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using ONITwitchCore.Config;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Patches;

public static class MainMenuPatches
{
	[HarmonyPatch(typeof(MainMenu), "OnSpawn")]
	public static class MainMenu_OnSpawn_Patch
	{
		[UsedImplicitly]
		public static void Postfix()
		{
			var credentials = CredentialsConfig.Instance.Credentials;
			ValidateNick(credentials.Nick);
			ValidateToken(credentials.Oauth);
		}

		private static void ValidateNick(string nick)
		{
			string errMsg = null;
			if (nick.IsNullOrWhiteSpace())
			{
				Log.Warn("Null or whitespace nick in credentials");
				errMsg = "The credentials file does not have a Twitch login name set";
			}
			else if (nick.Contains("/") || nick.Contains("\\"))
			{
				Log.Warn($"Nick contained a slash: {nick}");
				errMsg =
					"The Twitch nickname in the credentials file contained a slash.  The nickname should be <i>only</i> the name you use to log in to Twitch.";
			}

			if (!errMsg.IsNullOrWhiteSpace())
			{
				var toastGo = ToastManager.InstantiateToast("Invalid Credentials", errMsg);
				if ((toastGo != null) && toastGo.TryGetComponent(out Toast toast))
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
				errMsg = "Invalid OAuth token!\nThe OAuth token should be composed of only numbers and letters.";
			}
			else
			{
				// validate the token using the twitch endpoint
				const string twitchTokenValidateUri = "https://id.twitch.tv/oauth2/validate";

				var request = WebRequest.CreateHttp(twitchTokenValidateUri);
				var headers = new WebHeaderCollection
					{ { "Authorization", $"Bearer {oauth}" } };
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
							? "The OAuth token is invalid or has expired.  Please generate a new token following the instructions in the README."
							: "An unknown error occured when validating your OAuth token with Twitch.";
					}
					else
					{
						Log.Warn("Error validating oauth token with twitch.  No response.");
						errMsg =
							"An unknown error occured when validating your OAuth token with Twitch. (No response from server)";
					}
				}
			}

			if (!errMsg.IsNullOrWhiteSpace())
			{
				var toastGo = ToastManager.InstantiateToast("Invalid Credentials", errMsg);
				if ((toastGo != null) && toastGo.TryGetComponent(out Toast toast))
				{
					toast.HoverTime = 60f;
				}
			}
		}
	}
}
