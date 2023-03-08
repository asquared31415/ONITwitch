using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;

namespace ONITwitchCore.Config;

public class CredentialsConfig
{
	private static CredentialsConfig instance;

	public static CredentialsConfig Instance => instance ??= new CredentialsConfig();

	public Credentials Credentials;

	private CredentialsConfig()
	{
		try
		{
			var text = File.ReadAllText(TwitchModInfo.CredentialsPath);
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
				TwitchModInfo.CredentialsPath,
				JsonConvert.SerializeObject(Credentials, Formatting.Indented)
			);
		}
		catch (JsonException je)
		{
			Log.Warn("Invalid JSON for credentials");
			Log.Warn($"{je}");
			Credentials = new Credentials();
			File.WriteAllText(
				TwitchModInfo.CredentialsPath,
				JsonConvert.SerializeObject(Credentials, Formatting.Indented)
			);
			DialogUtil.MakeDialog(
				"Invalid Credentials",
				"The credentials file was broken and has been reset, please follow the instructions in the README",
				"Ok",
				null
			);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
}

public record struct Credentials([NotNull] string Nick, [NotNull] string Oauth)
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
