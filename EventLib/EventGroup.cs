using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace EventLib;

public class EventGroup
{
	private static Dictionary<Assembly, string> assemblyStaticIdMap;

	public readonly string Name;
	public int TotalWeight => weights.Values.Sum();

	public event Action<EventGroup> OnGroupChanged;

	private readonly Dictionary<EventInfo, int> weights = new();

	public EventGroup([NotNull] string name)
	{
		Name = name;
	}

	public static (EventInfo EventInfo, EventGroup Group) DefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		var eventNamespace = GetEventNamespace(callingAssembly);

		var group = new EventGroup(GetItemDefaultGroupName(eventNamespace, id));
		var eventInfo = new EventInfo(group, eventNamespace, id, friendlyName);
		group.weights.Add(eventInfo, weight);

		// note: we skip calling the group changed event because we know it was just created and has no subscribers
		return (eventInfo, group);
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
		if (assemblyStaticIdMap == null)
		{
			assemblyStaticIdMap = new Dictionary<Assembly, string>();
			foreach (var mod in Global.Instance.modManager.mods)
			{
				var modStaticID = mod.staticID;
				var loadedData = mod.loaded_mod_data;
				if (loadedData != null)
				{
					foreach (var assembly in loadedData.dlls)
					{
						assemblyStaticIdMap.Add(assembly, modStaticID);
					}
				}
			}
		}

		string modNamespace;
		if (assemblyStaticIdMap.TryGetValue(callingAssembly, out var staticId))
		{
			modNamespace = staticId;
		}
		else
		{
			Debug.LogWarning(
				$"[Twitch Integration] Unable to find a static ID for assembly {callingAssembly}, it will not be namespaced"
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

	private void InvokeOnChanged()
	{
		// make local copy to make sure that deregistering between null check and call doesnt happen
		var onChangedEvent = OnGroupChanged;
		onChangedEvent?.Invoke(this);
	}
}
