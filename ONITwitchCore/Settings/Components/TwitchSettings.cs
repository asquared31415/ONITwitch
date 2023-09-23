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

namespace ONITwitch.Settings.Components;

internal static class TwitchSettings
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
					Log.Debug($"Converted config: {config}");
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
					config = SettingsData.CreateDefault();
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

			Log.Debug($"Final config: {config}");

			return config;
		}
		catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
		{
			return SettingsData.CreateDefault();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return SettingsData.CreateDefault();
		}
	}

	internal class SettingsData
	{
		// TODO: make this configurable, but only in the file?
		public const string VotesPath = "votes.txt";

		public string ChannelName;

		// ReSharper disable once FieldCanBeMadeReadOnly.Global This is assigned by deserialization
		// Note: defaults here are set in CreateDefault, to work around serialization issues.
		public List<string> DisallowedDupeNames;

		public int LastOpenedSettingsVersion;

		public Danger MaxDanger;
		public Danger MinDanger;

		/// <summary>
		///     <see langword="true" /> if photosensitive mode is enabled. If photosensitive mode is enabled, visual effects should
		///     be reduced or disabled.
		/// </summary>
		public bool PhotosensitiveMode;

		public bool ShowToasts;
		public bool ShowVoteStartToasts;
		public bool UseTwitchNameColors;

		// default version for deserializing, must be overwritten
		public int Version;

		public int VoteCount;
		public float VoteDelay;
		public float VoteTime;

		[Obsolete("The default constructor is useless for serialization purposes, use CreateDefault instead")]
		internal SettingsData()
		{
		}

		/// <summary>
		///     Use this to create new instances of SettingsData. Works around bugs with deserializing lists with Newtonsoft.
		/// </summary>
		[NotNull]
		internal static SettingsData CreateDefault()
		{
			// This is intentionally the only blessed usage of the constructor
		#pragma warning disable CS0618
			var data = new SettingsData
		#pragma warning restore CS0618
			{
				Version = CurrentConfigVersion,
				ChannelName = "",
				// ReSharper disable StringLiteralTypo (bot names aren't normal words)
				DisallowedDupeNames = new List<string>
					{ "nightbot", "streamelements", "streamlabs", "fossabot", "moobot" },
				// ReSharper restore StringLiteralTypo
				// Default to v0 so that the "new settings" message always shows when
				// a default config is created.
				LastOpenedSettingsVersion = 0,
				MaxDanger = Danger.High,
				MinDanger = Danger.None,
				PhotosensitiveMode = false,
				ShowToasts = true,
				ShowVoteStartToasts = true,
				UseTwitchNameColors = true,
				VoteCount = 3,
				VoteDelay = 540,
				VoteTime = 60,
			};
			return data;
		}

		public override string ToString()
		{
			return
				$"SettingsData {{ Version = {Version}, LastOpenedSettingsVersion = {LastOpenedSettingsVersion}, " +
				$"ChannelName = {ChannelName}, VoteDelay = {VoteDelay}, VoteTime = {VoteTime}," +
				$" VoteCount = {VoteCount}, UseTwitchNameColors={UseTwitchNameColors}, ShowToasts = {ShowToasts}," +
				$" ShowVoteStartToasts = {ShowVoteStartToasts}, MinDanger = {MinDanger}, MaxDanger = {MaxDanger}," +
				$" PhotosensitiveMode = {PhotosensitiveMode}, DisallowedDupeNames = {string.Join(", ", DisallowedDupeNames)}}}";
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

	private class VersionedSave(int version, JObject saveData)
	{
		internal readonly JObject SaveData = saveData;
		internal readonly int Version = version;
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
				// ReSharper disable StringLiteralTypo
				["DisallowedDupeNames"] = new JArray("nightbot", "streamelements", "streamlabs", "fossabot", "moobot"),
				// ReSharper restore StringLiteralTypo
			};
			return output;
		}
	}

	[Obsolete("Internal for the merge lib", true)]
	[UsedImplicitly]
	[NotNull]
	// Entries may be added or removed to this dictionary freely. Users are advised that keys
	// may or may not exist in the future and to handle that appropriately.
	private static IReadOnlyDictionary<string, object> GetSettingsData()
	{
		var data = GetConfig();
		var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(
			JsonConvert.SerializeObject(data)
		);

		return d;
	}
}
