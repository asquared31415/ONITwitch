using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;

namespace ONITwitch.Config;

internal class CredentialsConfig
{
	private static CredentialsConfig instance;

	private const string CredentialsFileName = "SECRET_credentials.json";

	private static readonly string CredentialsPath = Path.Combine(TwitchModInfo.MainModFolder, CredentialsFileName);

	public static CredentialsConfig Instance => instance ??= new CredentialsConfig();

	public Credentials Credentials;

	private CredentialsConfig()
	{
		try
		{
			var text = File.ReadAllText(CredentialsPath);
			Credentials = JsonConvert.DeserializeObject<Credentials>(text);
			if (!Credentials.Oauth.StartsWith("oauth:"))
			{
				Credentials.Oauth = "oauth:" + Credentials.Oauth;
			}
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			Credentials = new Credentials();
			File.WriteAllText(
				CredentialsPath,
				JsonConvert.SerializeObject(Credentials, Formatting.Indented)
			);
		}
		catch (JsonException je)
		{
			Log.Warn("Invalid JSON for credentials");
			Log.Warn($"{je}");
			Credentials = new Credentials();
			File.WriteAllText(
				CredentialsPath,
				JsonConvert.SerializeObject(Credentials, Formatting.Indented)
			);
			DialogUtil.MakeDialog(
				STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.TITLE,
				STRINGS.ONITWITCH.UI.DIALOGS.INVALID_CREDENTIALS.BODY,
				UI.CONFIRMDIALOG.OK,
				null
			);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
}

internal record struct Credentials([NotNull] string Nick, [NotNull] string Oauth)
{
	public Credentials() : this("", "")
	{
	}

	public bool IsValid()
	{
		var invalid = string.IsNullOrWhiteSpace(Nick) || string.IsNullOrWhiteSpace(Oauth);
		return !invalid;
	}
}
