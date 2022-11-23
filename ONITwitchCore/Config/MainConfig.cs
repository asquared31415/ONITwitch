using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchCore.Patches;
using ONITwitchLib;

namespace ONITwitchCore.Config;

public class MainConfig
{
	private static MainConfig instance;
	public static MainConfig Instance => instance ??= new MainConfig();

	public ConfigData ConfigData { get; private set; }

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

			ConfigData = DeserializeConfig();

			if (ConfigData == null)
			{
				PauseMenuPatches.TwitchButtonInfo.isEnabled = false;
				if (PauseScreen.Instance != null)
				{
					PauseScreen.Instance.RefreshButtons();
				}
			}
		}
	}

	private static ConfigData DeserializeConfig()
	{
		try
		{
			var configText = File.ReadAllText(TwitchModInfo.ConfigPath);
			var config = JsonConvert.DeserializeObject<ConfigData>(configText);
			return config;
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			var config = new ConfigData();
			File.WriteAllText(TwitchModInfo.ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
			return config;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}

		return new ConfigData();
	}
}

public record struct ConfigData(
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
	public ConfigData() : this(
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
