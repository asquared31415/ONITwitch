using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using KMod;
using ONITwitchCore;
using ONITwitchCore.Config;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace EventLib;

public class EventGroup
{
	public static Dictionary<Assembly, string> AssemblyIdMap;

	[NotNull] public readonly string Name;
	public int TotalWeight => weights.Values.Sum();

	public event Action<EventGroup> OnGroupChanged;

	private readonly Dictionary<EventInfo, int> weights = new();

	private EventGroup([NotNull] string name)
	{
		Name = name;
	}

	[NotNull]
	public static EventGroup GetOrCreateGroup([NotNull] string name)
	{
		var existing = TwitchDeckManager.Instance.GetGroup(name);
		return existing ?? new EventGroup(name);
	}

	[MustUseReturnValue("The group should be added to the TwitchDeckManager to actually be useful")]
	public static (EventInfo EventInfo, EventGroup Group) DefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		return CommonDefaultSingleEventGroup(callingAssembly, id, weight, friendlyName);
	}

	[NotNull]
	public EventInfo AddEvent([NotNull] string id, int weight, [CanBeNull] string friendlyName = null)
	{
		var eventNamespace = GetEventNamespace(Assembly.GetCallingAssembly());

		// get the weight and name from the config if applicable
		var config = UserCommandConfigManager.Instance.GetConfig(eventNamespace, id);
		if (config != null)
		{
			if (config.FriendlyName != null)
			{
				friendlyName = config.FriendlyName;
			}

			weight = config.Weight;
		}

		// add the event to a different group if that was specified 
		if ((config?.GroupName != null) && (config.GroupName != Name))
		{
			var existingGroup = TwitchDeckManager.Instance.GetGroup(config.GroupName);
			if (existingGroup != null)
			{
				var eventInfo = new EventInfo(existingGroup, eventNamespace, id, friendlyName);
				existingGroup.AddEventInfoInternal(eventInfo, weight);

				return eventInfo;
			}
			else
			{
				// it was specified to be in a group that does not yet exist
				var group = GetOrCreateGroup(config.GroupName);
				var eventInfo = new EventInfo(group, eventNamespace, id, friendlyName);
				group.AddEventInfoInternal(eventInfo, weight);

				return eventInfo;
			}
		}

		// no group override in the config, just add it
		{
			var eventInfo = new EventInfo(this, eventNamespace, id, friendlyName);
			AddEventInfoInternal(eventInfo, weight);

			return eventInfo;
		}
	}

	public void SetWeight([NotNull] EventInfo eventInfo, int weight)
	{
		if (weights.ContainsKey(eventInfo))
		{
			weights[eventInfo] = weight;
			InvokeOnChanged();
		}
	}

	public void RemoveEvent([NotNull] EventInfo item)
	{
		if (weights.ContainsKey(item))
		{
			weights.Remove(item);
			InvokeOnChanged();
		}
	}

	[NotNull]
	public IReadOnlyDictionary<EventInfo, int> GetWeights()
	{
		// make copy of the dict to prevent weight modification
		// and then wrap it in a read only dict to make it clear to not write to it anyway
		return new ReadOnlyDictionary<EventInfo, int>(new Dictionary<EventInfo, int>(weights));
	}

	internal void AddEventInfoInternal([NotNull] EventInfo eventInfo, int weight)
	{
		weights[eventInfo] = weight;
		InvokeOnChanged();
	}

	private static string GetEventNamespace([NotNull] Assembly callingAssembly)
	{
		string modNamespace;
		if ((AssemblyIdMap != null) && AssemblyIdMap.TryGetValue(callingAssembly, out var staticId))
		{
			modNamespace = staticId;
		}
		else
		{
			// Warning for mod devs to be present even on the release versions 
			Log.Warn($"Unable to find a static ID for assembly {callingAssembly} to determine its namespace (did you try to register an event before the twitch mod's OnAllModsLoaded?)");
			modNamespace = "";
		}

		return modNamespace;
	}

	/// <summary>
	/// This is not stable
	/// </summary>
	[NotNull]
	public static string GetItemDefaultGroupName([NotNull] string eventNamespace, [NotNull] string id)
	{
		return $"__<nogroup>__{eventNamespace}.{id}_{eventNamespace.GetHashCode()}.{id.GetHashCode()}_";
	}

	public override string ToString()
	{
		return $"Group {Name}";
	}

	private void InvokeOnChanged()
	{
		// make local copy to make sure that deregistering between null check and call doesnt happen
		var onChangedEvent = OnGroupChanged;
		onChangedEvent?.Invoke(this);
	}

	internal static void RegisterStaticIdMap(IEnumerable<Mod> mods)
	{
		if (AssemblyIdMap != null)
		{
			Log.Debug("Attempting to initialize the assembly->static id map twice");
			Log.Debug(Environment.StackTrace);
		}
		else
		{
			Log.Debug("Initializing mod assembly map");
			AssemblyIdMap = new Dictionary<Assembly, string>();
			foreach (var registeredMod in mods)
			{
				if (registeredMod.IsEnabledForActiveDlc())
				{
					var loadedData = registeredMod.loaded_mod_data;
					if (loadedData != null)
					{
						var modStaticID = registeredMod.staticID;
						foreach (var assembly in loadedData.dlls)
						{
							AssemblyIdMap.Add(assembly, modStaticID);
						}
					}
				}
			}
		}
	}

	private static (EventInfo EventInfo, EventGroup Group) CommonDefaultSingleEventGroup(
		[NotNull] Assembly callingAssembly,
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var eventNamespace = GetEventNamespace(callingAssembly);

		var groupName = GetItemDefaultGroupName(eventNamespace, id);

		// get the group and weight and name from the config if applicable
		var config = UserCommandConfigManager.Instance.GetConfig(eventNamespace, id);
		if (config != null)
		{
			if (config.FriendlyName != null)
			{
				friendlyName = config.FriendlyName;
			}

			weight = config.Weight;

			if (config.GroupName != null)
			{
				groupName = config.GroupName;
			}
		}

		// if the group already exists (because of config) get it
		var existing = TwitchDeckManager.Instance.GetGroup(groupName);
		if (existing != null)
		{
			var eventInfo = new EventInfo(existing, eventNamespace, id, friendlyName);
			existing.AddEventInfoInternal(eventInfo, weight);

			return (eventInfo, existing);
		}
		else
		{
			var group = GetOrCreateGroup(groupName);
			var eventInfo = new EventInfo(group, eventNamespace, id, friendlyName);
			group.AddEventInfoInternal(eventInfo, weight);

			return (eventInfo, group);
		}
	}

	[Obsolete("Used as a cast helper for the reflection lib", true)]
	[UsedImplicitly]
	private static (object, object) InternalDefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		return CommonDefaultSingleEventGroup(callingAssembly, id, weight, friendlyName);
	}
}
