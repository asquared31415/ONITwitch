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

	// explicitly call the constructor so that the defaults are set
	public ConfigData ConfigData { get; private set; } = new();

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

			var config = DeserializeConfig();
			if (config.HasValue)
			{
				ConfigData = config.Value;
				if (ConfigData.MinDanger == Danger.Any)
				{
					Debug.LogWarning(
						"[Twitch Integration] Use of the Any danger (-1) as a min danger in config is not supported"
					);
					ConfigData = ConfigData with { MinDanger = Danger.None };
				}

				if (ConfigData.MaxDanger == Danger.Any)
				{
					Debug.LogWarning(
						"[Twitch Integration] Use of the Any danger (-1) as a max danger in config is not supported"
					);
					ConfigData = ConfigData with { MaxDanger = Danger.High };
				}
			}
			else
			{
				PauseMenuPatches.TwitchButtonInfo.isEnabled = false;
				if (PauseScreen.Instance != null)
				{
					PauseScreen.Instance.RefreshButtons();
				}
			}
		}
	}

	private static ConfigData? DeserializeConfig()
	{
		try
		{
			var configText = File.ReadAllText(TwitchModInfo.ConfigPath);
			var config = JsonConvert.DeserializeObject<ConfigData>(configText);

			// The old data has been removed and the new fields are the default for the *type*
			if (configText.Contains("DisableToasts"))
			{
				config.ShowToasts = true;
				config.ShowVoteStartToasts = true;
				WriteConfig(config);
			}

			return config;
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			var config = new ConfigData();
			WriteConfig(config);
			return config;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return null;
		}
	}

	private static void WriteConfig(ConfigData config)
	{
		File.WriteAllText(TwitchModInfo.ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
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
	bool ShowToasts,
	bool ShowVoteStartToasts,
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
		true,
		true,
		true
	)
	{
	}
}
