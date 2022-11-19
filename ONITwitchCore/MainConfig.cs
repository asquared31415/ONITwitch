using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchLib;

namespace ONITwitchCore;

public class MainConfig
{
	private static MainConfig instance;
	public static MainConfig Instance => instance ??= new MainConfig();

	public Config Config { get; private set; }

	private System.DateTime lastLoadTime = System.DateTime.MinValue;
	private readonly object loadLock = new();

	[NotNull] private readonly FileSystemWatcher configWatcher = new()
	{
		Path = TwitchModInfo.MainModFolder, NotifyFilter = NotifyFilters.LastWrite, Filter = TwitchModInfo.ConfigName,
	};

	private MainConfig()
	{
		LoadConfig();

		configWatcher.Changed += (_, _) => LoadConfig();
		configWatcher.EnableRaisingEvents = true;
	}

	private void LoadConfig()
	{
		lock (loadLock)
		{
			// don't reload if the last load was less than a half second ago 
			var now = System.DateTime.Now;
			if (now.Subtract(lastLoadTime).TotalSeconds < 0.5f)
			{
				return;
			}

			lastLoadTime = now;

			Config = DeserializeConfig();

			if (Config == null)
			{
				PauseMenuPatches.TwitchButtonInfo.isEnabled = false;
				if (PauseScreen.Instance != null)
				{
					PauseScreen.Instance.RefreshButtons();
				}
			}
		}
	}

	private static Config DeserializeConfig()
	{
		try
		{
			var configText = File.ReadAllText(TwitchModInfo.ConfigPath);
			var config = JsonConvert.DeserializeObject<Config>(configText);
			return config;
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			var config = new Config();
			File.WriteAllText(TwitchModInfo.ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
			return config;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}

		return new Config();
	}
}

public record struct Config(
	string Channel,
	float CyclesPerVote,
	float VoteTime,
	int NumVotes,
	Danger MinDanger,
	Danger MaxDanger,
	string VotesPath,
	string VoteHeader,
	bool DisableToasts,
	bool DisableVoteStartToasts,
	bool UseTwitchNameColor
)
{
	public Config() : this(
		"",
		2,
		60,
		3,
		Danger.None,
		Danger.High,
		"votes.txt",
		"Current Vote",
		false,
		true,
		true
	)
	{
	}
}
