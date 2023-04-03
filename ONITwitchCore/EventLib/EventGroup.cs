using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using KMod;
using ONITwitch.Config;
using ONITwitchLib.Logger;

namespace ONITwitch.EventLib;

/// <summary>
/// A group of associated <see cref="EventInfo"/>s with relative weights.
/// <see cref="EventInfo"/>s in an <see cref="EventGroup"/> will attempt to be spread out for variety.
/// </summary>
[PublicAPI]
public class EventGroup
{
	/// <summary>
	/// The name of the group.
	/// </summary>
	[PublicAPI] [NotNull] public readonly string Name;

	/// <summary>
	/// The total weight of the group.
	/// </summary>
	[PublicAPI]
	public int TotalWeight => weights.Values.Sum();

	/// <summary>
	/// An event that fires when the group is changed, called with the group that changed.
	/// </summary>
	[PublicAPI]
	public event Action<EventGroup> OnGroupChanged;

	/// <summary>
	/// Gets an existing <see cref="EventGroup"/> with a specified name, or creates it if it does not exist.
	/// </summary>
	/// <param name="name">The name of the <see cref="EventGroup"/> to get or create.</param>
	/// <returns>The group that was found or created.</returns>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public static EventGroup GetOrCreateGroup([NotNull] string name)
	{
		// be sure that the name is not null, even if people don't respect the api
		if (name == null)
		{
			throw new ArgumentNullException(nameof(name));
		}

		var existing = TwitchDeckManager.Instance.GetGroup(name);
		return existing ?? new EventGroup(name);
	}

	/// <summary>
	/// Creates an <see cref="EventInfo"/> with a unique <see cref="EventGroup"/> that has a default name and no other <see cref="EventInfo"/>s.
	/// </summary>
	/// <param name="id">The id of the <see cref="EventInfo"/> to create.</param>
	/// <param name="weight">The weight of the <see cref="EventInfo"/> to create.</param>
	/// <param name="friendlyName">The friendly name of the <see cref="EventInfo"/> to create.</param>
	/// <returns>The newly created <see cref="EventInfo"/> and its unique <see cref="EventGroup"/>.</returns>
	[PublicAPI]
	[MustUseReturnValue("The group should be added to the TwitchDeckManager to be used")]
	public static (EventInfo EventInfo, EventGroup Group) DefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		return CommonDefaultSingleEventGroup(callingAssembly, id, weight, friendlyName);
	}

	/// <summary>
	/// Creates a new <see cref="EventInfo"/> in this <see cref="EventGroup"/>.
	/// </summary>
	/// <param name="id">The id of the <see cref="EventInfo"/> to create.</param>
	/// <param name="weight">The weight of the <see cref="EventInfo"/> to create.</param>
	/// <param name="friendlyName">The friendly name of the <see cref="EventInfo"/> to create.</param>
	/// <returns>The newly created <see cref="EventInfo"/>.</returns>
	[PublicAPI]
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

	/// <summary>
	/// Sets the weight of a specified <see cref="EventInfo"/> in the group.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo"/> to change the weight for.</param>
	/// <param name="weight">The new weight.</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="weight"/> is less than 0.</exception>
	[PublicAPI]
	public void SetWeight([NotNull] EventInfo eventInfo, int weight)
	{
		if (weights.ContainsKey(eventInfo))
		{
			if (weight < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(weight), weight, "Weight must be non-negative.");
			}

			weights[eventInfo] = weight;
			InvokeOnChanged();
		}
	}

	/// <summary>
	/// Removes the specified <see cref="EventInfo"/> from the group.
	/// </summary>
	/// <param name="item">The <see cref="EventInfo"/> to remove.</param>
	[PublicAPI]
	public void RemoveEvent([NotNull] EventInfo item)
	{
		if (weights.ContainsKey(item))
		{
			weights.Remove(item);
			InvokeOnChanged();
		}
	}

	/// <summary>
	/// Gets a the weight of each <see cref="EventInfo"/> in the group.
	/// </summary>
	/// <returns>A read-only dictionary of each <see cref="EventInfo"/> and its corresponding weight.</returns>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public IReadOnlyDictionary<EventInfo, int> GetWeights()
	{
		// make copy of the dict to prevent weight modification
		// and then wrap it in a read only dict to make it clear to not write to it anyway
		return new ReadOnlyDictionary<EventInfo, int>(new Dictionary<EventInfo, int>(weights));
	}

	/// <summary>
	/// Displays a string representation of a group using its name.
	/// </summary>
	/// <returns>A string representation of the object.</returns>
	public override string ToString()
	{
		return $"Group {Name}";
	}

	private readonly Dictionary<EventInfo, int> weights = new();

	private EventGroup([NotNull] string name)
	{
		Name = name;
	}

	internal void AddEventInfoInternal([NotNull] EventInfo eventInfo, int weight)
	{
		weights[eventInfo] = weight;
		InvokeOnChanged();
	}

	internal static Dictionary<Assembly, string> AssemblyIdMap;

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
			Log.Warn(
				$"Unable to find a static ID for assembly {callingAssembly} to determine its namespace (did you try to register an event before the twitch mod's OnAllModsLoaded?)"
			);
			modNamespace = "";
		}

		return modNamespace;
	}

	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	internal static string GetItemDefaultGroupName([NotNull] string eventNamespace, [NotNull] string id)
	{
		return $"__<nogroup>__{eventNamespace}.{id}_{eventNamespace.GetHashCode()}.{id.GetHashCode()}_";
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

	[MustUseReturnValue]
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
	[PublicAPI(
		"This is not part of the public API. It exists solely for merged library internals. However, removing this is a breaking change."
	)]
	private static (object, object) InternalDefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		return CommonDefaultSingleEventGroup(callingAssembly, id, weight, friendlyName);
	}

	// `action` is just a wrapper for an `Action` stored on the merge lib side and passes the `this` of the merge lib instead
	[Obsolete("Used as a helper for the reflection lib", true)]
	[PublicAPI(
		"This is not part of the public API. It exists solely for merged library internals. However, removing this is a breaking change."
	)]
	private void AddMergeLibChangedListener([NotNull] System.Action action)
	{
		OnGroupChanged += _ => action();
	}
}
