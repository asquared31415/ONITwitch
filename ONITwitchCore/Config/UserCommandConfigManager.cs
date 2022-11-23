using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchLib;

namespace ONITwitchCore.Config;

public class UserCommandConfigManager
{
	private static UserCommandConfigManager instance;
	public static UserCommandConfigManager Instance => instance ??= new UserCommandConfigManager();

	private System.DateTime lastLoadTime = System.DateTime.MinValue;
	private readonly object loadLock = new();

	private const string CommandConfigName = "command_config.json";
	private static readonly string CommandConfigPath = Path.Combine(TwitchModInfo.MainModFolder, CommandConfigName);

	[NotNull] private readonly FileSystemWatcher configWatcher = new()
	{
		Path = TwitchModInfo.MainModFolder, NotifyFilter = NotifyFilters.LastWrite, Filter = CommandConfigName,
	};

	private UserCommandConfigManager()
	{
		Reload();

		configWatcher.Changed += (_, _) => Reload();
		configWatcher.EnableRaisingEvents = true;
	}

	private Dictionary<string, CommandConfig> userConfig = new();

	public void DEBUG_DumpCurrentConfig()
	{
		var data = JsonConvert.SerializeObject(userConfig, Formatting.Indented);
		Debug.Log(data);
	}

	public void Reload()
	{
		lock (loadLock)
		{
			// don't reload if the last load was less than a half second ago 
			var now = System.DateTime.Now;
			if (now.Subtract(lastLoadTime).TotalSeconds >= 0.5f)
			{
				lastLoadTime = now;

				try
				{
					var configText = File.ReadAllText(CommandConfigPath);
					userConfig = JsonConvert.DeserializeObject<Dictionary<string, CommandConfig>>(
						configText,
						new NestedDictionaryReader()
					);

					// convert all remaining objects to string, object dictionary
					foreach (var key in userConfig.Keys.ToList())
					{
						if (userConfig[key].Data is JObject obj)
						{
							userConfig[key].Data = obj.ToObject<Dictionary<string, object>>();
						}
					}
				}
				catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
				{
					// we can ignore no config being found, treat it like empty
					userConfig = new Dictionary<string, CommandConfig>();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					// insurance to try to not break too much
					userConfig = new Dictionary<string, CommandConfig>();
				}
			}

			DefaultCommands.ReloadData(userConfig);
		}
	}
}
