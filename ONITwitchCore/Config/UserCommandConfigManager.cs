using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchLib;
using DataManager = EventLib.DataManager;
using EventGroup = EventLib.EventGroup;
using EventManager = EventLib.EventManager;

// using ONITwitchLib;

namespace ONITwitchCore.Config;

public class UserCommandConfigManager
{
	public static UserCommandConfigManager Instance => instance ??= new UserCommandConfigManager();

	[CanBeNull]
	public CommandConfig GetConfig([NotNull] string eventNamespace, [NotNull] string eventId)
	{
		return userConfig.TryGetValue(eventNamespace, out var namespaceConfig)
			? namespaceConfig.TryGetValue(eventId, out var config) ? config : null
			: null;
	}

	private static UserCommandConfigManager instance;

	private System.DateTime lastLoadTime = System.DateTime.MinValue;
	private readonly object loadLock = new();

	private const string CommandConfigName = "command_config.json";
	internal static readonly string CommandConfigPath = Path.Combine(TwitchModInfo.MainModFolder, CommandConfigName);

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

	private void Reload()
	{
		lock (loadLock)
		{
			// don't reload if the last load was less than a half second ago 
			var now = System.DateTime.Now;
			if (now.Subtract(lastLoadTime).TotalSeconds >= 0.5f)
			{
				lastLoadTime = now;
				Debug.Log("[Twitch Integration] Reloading user config");

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

				ReloadEvents();
			}
		}
	}

	private void ReloadEvents()
	{
		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;
		foreach (var (namespaceId, namespaceInfo) in userConfig)
		{
			foreach (var (id, config) in namespaceInfo)
			{
				var eventId = eventInst.GetEventByID(namespaceId, id);
				if (eventId != null)
				{
					// friendly name and data can be updated always
					eventId.FriendlyName = config.FriendlyName;
					dataInst.SetDataForEvent(eventId, config.Data);

					var group = eventId.Group;
					// if the group did not move, just update the weight
					if (group.Name == config.GroupName)
					{
						group.SetWeight(eventId, config.Weight);
					}
					else
					{
						var groupName = config.GroupName ?? EventGroup.GetItemDefaultGroupName(
							eventId.EventNamespace,
							eventId.EventId
						);
						var newGroup = EventGroup.GetOrCreateGroup(groupName);
						eventId.MoveToGroup(newGroup, config.Weight);
						deckInst.AddGroup(newGroup);
					}
				}
			}
		}
	}
}
