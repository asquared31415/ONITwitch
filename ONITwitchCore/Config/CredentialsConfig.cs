using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitch.Config;

internal static class CredentialsConfig
{
	private const string CredentialsFileName = "SECRET_credentials.json";

	private static readonly string CredentialsPath = Path.Combine(TwitchModInfo.MainModFolder, CredentialsFileName);

	private static readonly Regex NickRegex = new("^[A-Za-z0-9][A-Za-z0-9_]*$");
	private static readonly Regex OauthRegex = new("^[0-9a-zA-Z]+$");

	internal static (Credentials credentials, string error) GetCredentials()
	{
		try
		{
			var text = File.ReadAllText(CredentialsPath);
			var credentials = JsonConvert.DeserializeObject<Credentials>(text);
			if (!credentials.Oauth.StartsWith("oauth:"))
			{
				credentials.Oauth = "oauth:" + credentials.Oauth;
			}

			// Check that the nickname is valid
			if (credentials.Nick.IsNullOrWhiteSpace())
			{
				Log.Warn("Null or whitespace nick in credentials");
				return (Credentials.CreateAnonymousCredentials(),
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.NO_LOGIN);
			}

			if (credentials.Nick.Contains("/") || credentials.Nick.Contains("\\"))
			{
				Log.Warn($"Nick contained a slash: {credentials.Nick}");
				return (Credentials.CreateAnonymousCredentials(),
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.NICK_CONTAINS_SLASH);
			}

			if (!NickRegex.IsMatch(credentials.Nick))
			{
				Log.Warn($"Nick did not match ^[A-Za-z0-9][A-Za-z0-9_]*$ {credentials.Nick}");
				return (Credentials.CreateAnonymousCredentials(),
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.INVALID_NICK);
			}

			// Check that the oauth token is both structurally correct and is valid according to Twitch
			// strip the oauth: prefix for all of the processing
			var oauth = credentials.Oauth;
			if (oauth.IndexOf("oauth:", StringComparison.Ordinal) != -1)
			{
				oauth = oauth.Substring("oauth:".Length);
			}

			if (!OauthRegex.IsMatch(oauth))
			{
				Log.Warn("oauth did not match the regex `[0-9a-zA-Z]+`");
				return (Credentials.CreateAnonymousCredentials(),
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.MALFORMED_OAUTH);
			}

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
					return (Credentials.CreateAnonymousCredentials(),
						httpResponse.StatusCode == HttpStatusCode.Unauthorized
							? STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.EXPIRED_OAUTH
							: STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.UNKNOWN_OAUTH_ERR);
				}

				Log.Warn("Error validating oauth token with twitch.  No response.");
				return (Credentials.CreateAnonymousCredentials(),
					STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.CONNECTION_OAUTH_ERR);
			}

			// validation passed, return the credentials unmodified with no error
			return (credentials, null);
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			var credentials = Credentials.CreateAnonymousCredentials();
			File.WriteAllText(
				CredentialsPath,
				JsonConvert.SerializeObject(credentials, Formatting.Indented)
			);
			return (credentials, STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.NO_LOGIN);
		}
		catch (JsonException je)
		{
			Log.Warn("Invalid JSON for credentials");
			Log.Warn($"{je}");
			var credentials = Credentials.CreateAnonymousCredentials();
			File.WriteAllText(
				CredentialsPath,
				JsonConvert.SerializeObject(credentials, Formatting.Indented)
			);
			return (credentials, STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.INVALID_FILE);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			var credentials = Credentials.CreateAnonymousCredentials();
			return (credentials, "An unexpected error occurred when loading credentials.");
		}
	}
}

internal record struct Credentials([NotNull] string Nick, [NotNull] string Oauth)
{
	public Credentials() : this("", "")
	{
	}

	public static Credentials CreateAnonymousCredentials()
	{
		return new Credentials($"justinfan{ThreadRandom.Next(100000)}", "oauth:XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
	}
}
