using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventManager = EventLib.EventManager;

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

	private Dictionary<string, Dictionary<string, CommandConfig>> userConfig = new();

	public void DEBUG_DumpCurrentConfig()
	{
		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;
		var data = new Dictionary<string, Dictionary<string, CommandConfig>>();

		foreach (var group in deckInst.GetGroups())
		{
			foreach (var (eventInfo, weight) in group.GetWeights())
			{
				var eventNamespace = eventInfo.EventNamespace;
				var eventId = eventInfo.EventId;

				var config = new CommandConfig
				{
					FriendlyName = eventInfo.FriendlyName,
					Data = dataInst.GetDataForEvent(eventInfo),
					Weight = weight,
					GroupName = group.Name,
				};
				if (data.TryGetValue(eventNamespace, out var namespaceEvents))
				{
					namespaceEvents[eventId] = config;
				}
				else
				{
					data[eventNamespace] = new Dictionary<string, CommandConfig> { [eventId] = config };
				}
			}
		}

		var ser = JsonConvert.SerializeObject(data, Formatting.None);
		Debug.Log(ser);
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
					userConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, CommandConfig>>>(
						configText,
						new NestedDictionaryReader()
					) ?? new Dictionary<string, Dictionary<string, CommandConfig>>();

					// convert all remaining objects to string, object dictionary
					foreach (var idNamespace in userConfig.Keys.ToList())
					{
						foreach (var id in userConfig[idNamespace].Keys.ToList())
						{
							if (userConfig[idNamespace][id].Data is JObject obj)
							{
								userConfig[idNamespace][id].Data = obj.ToObject<Dictionary<string, object>>();
							}
						}
					}
				}
				catch (IOException ie) when (ie is DirectoryNotFoundException or FileNotFoundException)
				{
					// we can ignore no config being found, treat it like empty
					userConfig = new Dictionary<string, Dictionary<string, CommandConfig>>();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					// insurance to try to not break too much
					userConfig = new Dictionary<string, Dictionary<string, CommandConfig>>();
				}

				DEBUG_DumpCurrentConfig();
				DefaultCommands.ReloadData(userConfig);
			}
		}
	}
}
