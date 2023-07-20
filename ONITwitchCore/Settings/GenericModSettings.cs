using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;

namespace ONITwitch.Settings;

internal static class GenericModSettings
{
	public const int CurrentConfigVersion = 2;

	[CanBeNull] private static SettingsData configData;

	[NotNull]
	public static SettingsData GetConfig()
	{
		if (configData == null)
		{
			configData = LoadConfig();
			OnSettingsChanged(configData);
		}

		return configData;
	}

	internal static void SetConfig([NotNull] SettingsData newData)
	{
		configData = newData;
		OnSettingsChanged(newData);
		SaveConfig(newData);
	}

	// Default init with an empty function so that it can't NRE
	[NotNull] public static event Action<SettingsData> OnSettingsChanged = _ => { };

	private static void SaveConfig(SettingsData toSave)
	{
		var ser = JsonConvert.SerializeObject(toSave, Formatting.Indented);
		File.WriteAllText(TwitchModInfo.ConfigPath, ser);
	}

	[NotNull]
	private static SettingsData LoadConfig()
	{
		try
		{
			Directory.CreateDirectory(TwitchModInfo.ConfigFolder);
			var configText = File.ReadAllText(TwitchModInfo.ConfigPath);
			var configObject = JsonConvert.DeserializeObject<JObject>(configText);

			var configVersion = 0;
			if (configObject.TryGetValue("Version", out var version))
			{
				configVersion = version.ToObject<int>();
			}

			SettingsData config;
			switch (configVersion)
			{
				// update and re-save any outdated config
				case < CurrentConfigVersion:
				{
					config = SaveConverters.ConvertSaveToLatest(configObject).ToObject<SettingsData>();
					SaveConfig(config);
					break;
				}
				case CurrentConfigVersion:
				{
					config = configObject.ToObject<SettingsData>();
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

					Log.Warn($"Found future config version {configVersion}");
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

	public static void SaveConverterTests()
	{
		var configText = @"{
			""Version"": 1,
			""ChannelName"": ""asquared31415"",
			""VoteDelay"": 50.0,
			""VoteTime"": 20.0,
			""VoteCount"": 3,
			""UseTwitchNameColors"": true,
			""ShowToasts"": true,
			""ShowVoteStartToasts"": true,
			""MinDanger"": 0,
			""MaxDanger"": 5
		}";

		var o = JsonConvert.DeserializeObject<JObject>(configText);
		SaveConverters.ConvertSaveToLatest(o);

		configText = @"{
			""Channel"": ""asquared31415"",
			""VoteDelay"": 50.0,
			""VoteTime"": 20.0,
			""VoteCountAAA"": 3,
			""ShowToasts"": true,
			""MinDanger"": 100,
			""MaxDanger"": 3
		}";
		o = JsonConvert.DeserializeObject<JObject>(configText);
		SaveConverters.ConvertSaveToLatest(o);
	}

	internal class SettingsData
	{
		// TODO: make this configurable, but only in the file?
		public const string VotesPath = "votes.txt";

		public string ChannelName = "";

		// Default with a list of well known bots
		// ReSharper disable once FieldCanBeMadeReadOnly.Global This is assigned by deserialization
		public List<string> DisallowedDupeNames =
			new() { "nightbot", "streamelements", "streamlabs", "fossabot", "moobot" };

		public int LastOpenedSettingsVersion;

		public Danger MaxDanger = Danger.High;
		public Danger MinDanger = Danger.None;

		/// <summary>
		///     <see langword="true" /> if photosensitive mode is enabled. If photosensitive mode is enabled, visual effects should
		///     be reduced or disabled.
		/// </summary>
		public bool PhotosensitiveMode = false;

		public bool ShowToasts = true;
		public bool ShowVoteStartToasts = true;
		public bool UseTwitchNameColors = true;

		// default version for deserializing, must be overwritten
		public int Version = 0;

		public int VoteCount = 3;
		public float VoteDelay = 600;
		public float VoteTime = 60;

		public override string ToString()
		{
			return
				$"SettingsData {{ Version = {Version}, LastOpenedSettingsVersion = {LastOpenedSettingsVersion}, " +
				$"ChannelName = {ChannelName}, VoteDelay = {VoteDelay}, VoteTime = {VoteTime}," +
				$" VoteCount = {VoteCount}, UseTwitchNameColors={UseTwitchNameColors}, ShowToasts = {ShowToasts}," +
				$" ShowVoteStartToasts = {ShowVoteStartToasts}, MinDanger = {MinDanger}, MaxDanger = {MaxDanger}," +
				$" PhotosensitiveMode = {PhotosensitiveMode}, DisallowedDupeNames = {DisallowedDupeNames}}}";
		}
	}

	private static class SaveConverters
	{
		// version -> converter mapping
		[NotNull] private static readonly Dictionary<int, ISaveConverter> Converters = new()
			{ { 0, new V0Converter() }, { 1, new V1Converter() } };

		public static JObject ConvertSaveToLatest([NotNull] JObject saved)
		{
			Log.Debug($"Input save {saved}");
			// default to 0 if it cannot be found
			// this will erase most useless data in the V0->V1 migration anyway
			var inVersion = saved.TryGetValue("Version", out var val)
				? val.ToObject<int>()
				: 0;

			while (inVersion < CurrentConfigVersion)
			{
				Log.Info($"Converting save from version {inVersion}");
				var converted = ConvertVersioned(new VersionedSave(inVersion, saved));
				Log.Debug($"Converted save to version {converted.Version}: {converted.SaveData}");
				inVersion = converted.Version;
				saved = converted.SaveData;
			}

			Log.Debug($"Final save: {saved}");
			return saved;
		}

		private static VersionedSave ConvertVersioned([NotNull] VersionedSave save)
		{
			if (Converters.TryGetValue(save.Version, out var converter))
			{
				var newVersion = converter.OutputVersion;
				var converted = converter.Convert(save.SaveData);
				return new VersionedSave(newVersion, converted);
			}

			throw new ArgumentException($"No save converter found for input version {save.Version}");
		}
	}

	private class VersionedSave
	{
		internal readonly JObject SaveData;
		internal readonly int Version;

		public VersionedSave(int version, JObject saveData)
		{
			Version = version;
			SaveData = saveData;
		}
	}

	private interface ISaveConverter
	{
		int OutputVersion { get; }

		JObject Convert(JObject input);
	}

	/// <summary>
	///     V1 format: {
	///     Version: 1,
	///     ChannelName: string,
	///     VoteDelay: float,
	///     VoteTime: float,
	///     VoteCount: int,
	///     UseTwitchNameColors: bool,
	///     ShowToasts: bool,
	///     ShowVoteStartToasts: bool,
	///     MinDanger: int 0..=5,
	///     MaxDanger: int 0..=5,
	///     }
	/// </summary>
	private class V0Converter : ISaveConverter
	{
		public int OutputVersion => 1;

		public JObject Convert(JObject input)
		{
			// try to rescue the channel name from legacy saves
			// VoteDelay: float also exists in legacy saves, but it's unchanged
			// otherwise everything else is lost, there was too much changing

			var converted = new JObject
			{
				["Version"] = 1,
				["VoteDelay"] = input["VoteDelay"],
				["VoteTime"] = 60.0f,
				["VoteCount"] = 3,
				["UseTwitchNameColors"] = false,
				["ShowToasts"] = true,
				["ShowVoteStartToasts"] = true,
				["MinDanger"] = (int) Danger.None,
				["MaxDanger"] = (int) Danger.High,
			};

			if (input.TryGetValue("Channel", out var channel))
			{
				if (channel.Type == JTokenType.String)
				{
					converted["ChannelName"] = channel.ToObject<string>();
				}
			}

			return converted;
		}
	}

	/// <summary>
	///     V2 format: {
	///     Version: 1,
	///     LastOpenedSettingsVersion: int,
	///     ChannelName: string,
	///     VoteDelay: float,
	///     VoteTime: float,
	///     VoteCount: int,
	///     UseTwitchNameColors: bool,
	///     ShowToasts: bool,
	///     ShowVoteStartToasts: bool,
	///     MinDanger: int 0..=5,
	///     MaxDanger: int 0..=5,
	///     PhotosensitiveMode: bool
	///     }
	///     Adds
	///     LastOpenedSettingsVersion: int
	///     PhotosensitiveMode: bool
	///     DisallowedDupeNames: List{string}
	/// </summary>
	private class V1Converter : ISaveConverter
	{
		public int OutputVersion => 2;

		public JObject Convert(JObject input)
		{
			var output = new JObject(input)
			{
				["Version"] = 2,
				// default to no known last version, this means we will display
				// an icon for V2
				["LastOpenedSettingsVersion"] = 0,
				["PhotosensitiveMode"] = false,
				["DisallowedDupeNames"] = new JArray("nightbot", "streamelements", "streamlabs", "fossabot", "moobot"),
			};
			return output;
		}
	}
}
