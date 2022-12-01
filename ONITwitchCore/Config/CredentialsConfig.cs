using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;

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
			File.WriteAllText(TwitchModInfo.CredentialsPath, JsonConvert.SerializeObject(Credentials, Formatting.Indented));
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
}

public record struct Credentials([NotNull] string Nick, [NotNull] string Oauth)
{
	public Credentials() : this("", "") { }

	public bool IsValid()
	{
		var invalid = string.IsNullOrWhiteSpace(Nick) || string.IsNullOrWhiteSpace(Oauth);
		return !invalid;
	}
}
