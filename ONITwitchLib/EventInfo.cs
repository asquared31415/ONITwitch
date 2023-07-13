using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Core;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

/// <summary>
///     Represents an event that is known to the <see cref="EventManager" />.
/// </summary>
[PublicAPI]
public class EventInfo
{
	private readonly Action<Func<object, bool>> addConditionDelegate;
	private readonly Action<Action<object>> addListenerDelegate;
	private readonly Func<object, bool> checkConditionDelegate;
	private readonly AccessTools.FieldRef<object, string> friendlyNameRef;
	private readonly Func<int?> getDangerDelegate;
	private readonly Func<string> getEventIdDelegate;
	private readonly Func<string> getEventNamespaceDelegate;
	private readonly Func<object> getGroupDelegate;

	private readonly Func<string> getIdDelegate;
	private readonly Action<Action<object>> removeListenerDelegate;
	private readonly Action<int?> setDangerDelegate;
	private readonly Action<object> triggerDelegate;

	internal EventInfo(object instance)
	{
		EventInfoInstance = instance;

		getIdDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(CoreTypes.EventInfoType, "Id"),
			EventInfoInstance
		);
		friendlyNameRef = AccessTools.FieldRefAccess<string>(CoreTypes.EventInfoType, "FriendlyName");
		getEventNamespaceDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(CoreTypes.EventInfoType, "EventNamespace"),
			EventInfoInstance
		);
		getEventIdDelegate = DelegateUtil.CreateDelegate<Func<string>>(
			AccessTools.PropertyGetter(CoreTypes.EventInfoType, "EventId"),
			EventInfoInstance
		);
		getGroupDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.PropertyGetter(CoreTypes.EventInfoType, "Group"),
			EventInfoInstance,
			CoreTypes.EventGroupType
		);
		getDangerDelegate = DelegateUtil.CreateDelegate<Func<int?>>(
			AccessTools.Method(CoreTypes.EventInfoType, "GetDangerInt", new Type[] { }),
			EventInfoInstance
		);
		setDangerDelegate = DelegateUtil.CreateDelegate<Action<int?>>(
			AccessTools.Method(CoreTypes.EventInfoType, "SetDangerInt", new[] { typeof(int?) }),
			EventInfoInstance
		);
		addListenerDelegate = DelegateUtil.CreateDelegate<Action<Action<object>>>(
			AccessTools.Method(CoreTypes.EventInfoType, "AddListener", new[] { typeof(Action<object>) }),
			EventInfoInstance
		);
		removeListenerDelegate = DelegateUtil.CreateDelegate<Action<Action<object>>>(
			AccessTools.Method(CoreTypes.EventInfoType, "RemoveListener", new[] { typeof(Action<object>) }),
			EventInfoInstance
		);
		triggerDelegate = DelegateUtil.CreateDelegate<Action<object>>(
			AccessTools.Method(CoreTypes.EventInfoType, "Trigger", new[] { typeof(object) }),
			EventInfoInstance
		);
		addConditionDelegate = DelegateUtil.CreateDelegate<Action<Func<object, bool>>>(
			AccessTools.Method(CoreTypes.EventInfoType, "AddCondition", new[] { typeof(Func<object, bool>) }),
			EventInfoInstance
		);
		checkConditionDelegate = DelegateUtil.CreateDelegate<Func<object, bool>>(
			AccessTools.Method(CoreTypes.EventInfoType, "CheckCondition", new[] { typeof(object) }),
			EventInfoInstance
		);
	}

	/// <summary>
	///     The full namespaced ID of the <see cref="EventInfo" />.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string Id => getIdDelegate();

	/// <summary>
	///     The friendly name of the <see cref="EventInfo" />.
	/// </summary>
	[PublicAPI]
	[CanBeNull]
	public string FriendlyName
	{
		get => friendlyNameRef(EventInfoInstance);
		set => friendlyNameRef(EventInfoInstance) = value;
	}

	/// <summary>
	///     The namespace of the <see cref="EventInfo" />.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string EventNamespace => getEventNamespaceDelegate();

	/// <summary>
	///     The ID of the <see cref="EventInfo" />, without the <see cref="EventNamespace" />.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string EventId => getEventIdDelegate();

	/// <summary>
	///     The <see cref="EventGroup" /> of the <see cref="EventInfo" />.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public EventGroup Group => new(getGroupDelegate());

	/// <summary>
	///     The <see cref="ONITwitchLib.Danger" /> of the <see cref="EventInfo" />.
	/// </summary>
	[PublicAPI]
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

	internal object EventInfoInstance { get; }

	/// <summary>
	///     Adds an <see cref="System.Action{T}" /> that is invoked with the event's data when the event is triggered.
	/// </summary>
	/// <param name="listener">The action to invoke when the event is triggered.</param>
	/// <seealso cref="DataManager" />
	/// <seealso cref="Trigger" />
	[PublicAPI]
	public void AddListener([NotNull] Action<object> listener)
	{
		addListenerDelegate(listener);
	}

	/// <summary>
	///     Removes an <see cref="System.Action{T}" /> from the list of actions that are run when an event is triggered, if it
	///     exists.
	/// </summary>
	/// <param name="listener">The action to remove.</param>
	/// <seealso cref="Trigger" />
	[PublicAPI]
	public void RemoveListener([NotNull] Action<object> listener)
	{
		removeListenerDelegate(listener);
	}

	/// <summary>
	///     Triggers the event with the specified data by calling each registered listener.
	///     Callers are expected to provide the correct type and values of data for this <see cref="EventInfo" />.
	///     The correct data can typically be found in the <see cref="DataManager" />.
	/// </summary>
	/// <param name="data">The data to call each listener with.</param>
	/// <seealso cref="AddListener" />
	[PublicAPI]
	public void Trigger(object data)
	{
		triggerDelegate(data);
	}

	/// <summary>
	///     Adds a condition to the event that should be run to determine if the event should run.
	/// </summary>
	/// <param name="condition">
	///     A function that takes an object parameter to be called with the event's data,
	///     and returns <see langword="true" /> if the event should be run and <see langword="false" /> if it should not.
	/// </param>
	/// <seealso cref="Trigger" />
	/// <seealso cref="DataManager" />
	/// <seealso cref="CheckCondition" />
	[PublicAPI]
	public void AddCondition([NotNull] Func<object, bool> condition)
	{
		addConditionDelegate(condition);
	}

	/// <summary>
	///     Checks whether an event should be run by invoking each of its conditions and returning <see langword="false" />
	///     if any of the conditions return <see langword="false" />.
	/// </summary>
	/// <param name="data">The data to be passed to each condition.</param>
	/// <returns><see langword="false" /> if any of the conditions return false, otherwise <see langword="true" />.</returns>
	/// <seealso cref="AddCondition" />
	/// <seealso cref="Trigger" />
	[PublicAPI]
	public bool CheckCondition(object data)
	{
		return checkConditionDelegate(data);
	}

	/// <summary>
	///     Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	/// <seealso cref="FriendlyName" />
	/// <seealso cref="Id" />
	/// <seealso cref="EventNamespace" />
	/// <seealso cref="EventId" />
	public override string ToString()
	{
		return EventInfoInstance.ToString();
	}
}
