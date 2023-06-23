using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;

namespace ONITwitch.Settings;

internal static class GenericModSettings
{
	public static SettingsData Data
	{
		get => data;
		internal set
		{
			data = value;
			SaveConfig(data);
		}
	}

	public const int CurrentConfigVersion = 1;

	internal class SettingsData
	{
		// default version for deserializing, must be overwritten
		public int Version = 0;

		public string ChannelName = "";
		public float VoteDelay = 600;
		public float VoteTime = 60;
		public int VoteCount = 3;
		public bool UseTwitchNameColors = true;
		public bool ShowToasts = true;
		public bool ShowVoteStartToasts = true;

		public Danger MinDanger = Danger.None;
		public Danger MaxDanger = Danger.High;

		// file only config
		public const string VotesPath = "votes.txt";

		public override string ToString()
		{
			return
				$"SettingsData {{ Version = {Version}, ChannelName = {ChannelName}, VoteDelay = {VoteDelay}, VoteTime = {VoteTime}," +
				$" VoteCount = {VoteCount}, UseTwitchNameColors={UseTwitchNameColors}, ShowToasts = {ShowToasts}," +
				$" ShowVoteStartToasts = {ShowVoteStartToasts}, MinDanger = {MinDanger}, MaxDanger = {MaxDanger}," +
				$" VotesPath = {VotesPath}}}";
		}
	}

	private static void SaveConfig(SettingsData toSave)
	{
		var ser = JsonConvert.SerializeObject(toSave, Formatting.Indented);
		File.WriteAllText(TwitchModInfo.ConfigPath, ser);
	}

	private static SettingsData data = LoadConfig();

	private static SettingsData LoadConfig()
	{
		try
		{
			Directory.CreateDirectory(TwitchModInfo.ConfigFolder);
			var configText = File.ReadAllText(TwitchModInfo.ConfigPath);
			var config = JsonConvert.DeserializeObject<SettingsData>(configText);
			switch (config.Version)
			{
				case <= 0:
				{
					Log.Debug("Migrating V0 config");
					config = MigrateV0Config(JsonConvert.DeserializeObject<JObject>(configText));

					try
					{
						File.Delete(TwitchModInfo.ConfigPath);
					}
					catch
					{
						// ignore all exceptions from IO crap here, deleting is a best effort
					}

					SaveConfig(config);
					break;
				}
				case CurrentConfigVersion:
				{
					break;
				}
				case > CurrentConfigVersion:
				{
					try
					{
						var backupPath = Path.Combine(TwitchModInfo.MainModFolder, "config_bak.json");
						if (File.Exists(backupPath))
						{
							File.Delete(backupPath);
						}

						File.Copy(TwitchModInfo.ConfigPath, backupPath);
					}
					catch (Exception)
					{
						// ignored
					}

					Log.Warn($"Found future config version {config.Version}");
					DialogUtil.MakeDialog(
						STRINGS.ONITWITCH.UI.DIALOGS.UNKNOWN_SAVE.TITLE,
						STRINGS.ONITWITCH.UI.DIALOGS.UNKNOWN_SAVE.BODY,
						UI.CONFIRMDIALOG.OK,
						null
					);
					config = new SettingsData();
					break;
				}
			}

			if (config.MaxDanger < config.MinDanger)
			{
				Log.Info(
					$"Invalid danger min/max: {config.MinDanger}/{config.MaxDanger} resetting min to {Danger.None}"
				);
				config.MinDanger = Danger.None;
				SaveConfig(config);
			}

			return config;
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			return new SettingsData();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return new SettingsData();
		}
	}

	private static SettingsData MigrateV0Config(JObject oldConfig)
	{
		// try to rescue certain settings
		var config = new SettingsData();

		if (oldConfig.TryGetValue("Channel", out var channel))
		{
			if (channel.Type == JTokenType.String)
			{
				config.ChannelName = channel.ToObject<string>();
			}
		}

		if (oldConfig.TryGetValue("VoteTime", out var voteTime))
		{
			if (voteTime.Type == JTokenType.Float)
			{
				config.VoteTime = voteTime.ToObject<float>();
			}
		}

		return config;
	}
}
