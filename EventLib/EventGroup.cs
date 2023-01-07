using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using KMod;

namespace EventLib;

public class EventGroup
{
	public static Dictionary<Assembly, string> AssemblyIdMap;

	public readonly string Name;
	public int TotalWeight => weights.Values.Sum();

	public event Action<EventGroup> OnGroupChanged;

	private readonly Dictionary<EventInfo, int> weights = new();

	public EventGroup([NotNull] string name)
	{
		Name = name;
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
		var callingAssembly = Assembly.GetCallingAssembly();
		var eventNamespace = GetEventNamespace(callingAssembly);

		var eventInfo = new EventInfo(this, eventNamespace, id, friendlyName);
		weights[eventInfo] = weight;

		InvokeOnChanged();

		return eventInfo;
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
			Debug.LogWarning(
				$"[Twitch Integration] Unable to find a static ID for assembly {callingAssembly} to determine its namespace (did you try to register an event too soon?)"
			);
			modNamespace = "";
		}

		return modNamespace;
	}

	/// <summary>
	/// This is not stable
	/// </summary>
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

	/// <summary>
	/// Don't call this
	/// </summary>
	public static void RegisterStaticIdMap(IEnumerable<Mod> mods)
	{
		if (AssemblyIdMap != null)
		{
			Debug.LogWarning("[Twitch Integration] Attempting to initialize the assembly->static id map twice");
			Debug.LogWarning(Environment.StackTrace);
		}
		else
		{
			Debug.Log("[Twitch Integration] Initializing mod assembly map");
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

			foreach (var (key, value) in AssemblyIdMap)
			{
				Debug.Log($"assembly {key}: {value}");
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

		var group = new EventGroup(GetItemDefaultGroupName(eventNamespace, id));
		var eventInfo = new EventInfo(group, eventNamespace, id, friendlyName);
		group.weights.Add(eventInfo, weight);

		// note: we skip calling the group changed event because we know it was just created and has no subscribers
		return (eventInfo, group);
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
