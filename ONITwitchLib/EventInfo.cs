using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
public class EventInfo
{
	[NotNull] public string Id => getIdDelegate();

	[CanBeNull]
	public string FriendlyName
	{
		get => friendlyNameRef(EventInfoInstance);
		set => friendlyNameRef(EventInfoInstance) = value;
	}

	[NotNull] public string EventNamespace => getEventNamespaceDelegate();
	[NotNull] public string EventId => getEventIdDelegate();

	[NotNull] public EventGroup Group => new(getGroupDelegate());

	[CanBeNull]
	public Danger? Danger
	{
		get
		{
			var dangerInt = getDangerDelegate();
			return dangerInt.HasValue ? (Danger) dangerInt.Value : null;
		}
		set
		{
			var intDanger = value.HasValue ? (int?) value.Value : null;
			setDangerDelegate(intDanger);
		}
	}

	public void AddListener([NotNull] Action<object> listener)
	{
		addListenerDelegate(listener);
	}

	public void RemoveListener([NotNull] Action<object> listener)
	{
		removeListenerDelegate(listener);
	}

	public void Trigger(object data)
	{
		triggerDelegate(data);
	}

	public void AddCondition([NotNull] Func<object, bool> condition)
	{
		addConditionDelegate(condition);
	}

	public bool CheckCondition(object data)
	{
		return checkConditionDelegate(data);
	}

	/// <summary>
	/// Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	public override string ToString()
	{
		return EventInfoInstance.ToString();
	}

	internal object EventInfoInstance { get; }

	private readonly Func<string> getIdDelegate;
	private readonly AccessTools.FieldRef<object, string> friendlyNameRef;
	private readonly Func<string> getEventNamespaceDelegate;
	private readonly Func<string> getEventIdDelegate;
	private readonly Func<object> getGroupDelegate;
	private readonly Func<int?> getDangerDelegate;
	private readonly Action<int?> setDangerDelegate;
	private readonly Action<Action<object>> addListenerDelegate;
	private readonly Action<Action<object>> removeListenerDelegate;
	private readonly Action<object> triggerDelegate;
	private readonly Action<Func<object, bool>> addConditionDelegate;
	private readonly Func<object, bool> checkConditionDelegate;

	internal EventInfo(object instance)
	{
		EventInfoInstance = instance;

		getIdDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(EventInterface.EventInfoType, "Id"),
			EventInfoInstance
		);
		friendlyNameRef = AccessTools.FieldRefAccess<string>(EventInterface.EventInfoType, "FriendlyName");
		getEventNamespaceDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(EventInterface.EventInfoType, "EventNamespace"),
			EventInfoInstance
		);
		getEventIdDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(EventInterface.EventInfoType, "EventId"),
			EventInfoInstance
		);
		getGroupDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.PropertyGetter(EventInterface.EventInfoType, "Group"),
			EventInfoInstance,
			EventInterface.EventGroupType
		);
		getDangerDelegate = DelegateUtil.CreateDelegate<Func<int?>>(
			AccessTools.Method(EventInterface.EventInfoType, "GetDangerInt"),
			EventInfoInstance
		);
		setDangerDelegate = DelegateUtil.CreateDelegate<Action<int?>>(
			AccessTools.Method(EventInterface.EventInfoType, "SetDangerInt"),
			EventInfoInstance
		);
		addListenerDelegate = DelegateUtil.CreateDelegate<Action<Action<object>>>(
			AccessTools.Method(EventInterface.EventInfoType, "AddListener"),
			EventInfoInstance
		);
		removeListenerDelegate = DelegateUtil.CreateDelegate<Action<Action<object>>>(
			AccessTools.Method(EventInterface.EventInfoType, "RemoveListener"),
			EventInfoInstance
		);
		triggerDelegate = DelegateUtil.CreateDelegate<Action<object>>(
			AccessTools.Method(EventInterface.EventInfoType, "Trigger"),
			EventInfoInstance
		);
		addConditionDelegate = DelegateUtil.CreateDelegate<Action<Func<object, bool>>>(
			AccessTools.Method(EventInterface.EventInfoType, "AddCondition"),
			EventInfoInstance
		);
		checkConditionDelegate = DelegateUtil.CreateDelegate<Func<object, bool>>(
			AccessTools.Method(EventInterface.EventInfoType, "CheckCondition"),
			EventInfoInstance
		);
	}
}
