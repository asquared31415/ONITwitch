using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;

namespace ONITwitchCore;

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
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			Credentials = new Credentials();
			File.WriteAllText(TwitchModInfo.CredentialsPath, JsonConvert.SerializeObject(Credentials));
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
