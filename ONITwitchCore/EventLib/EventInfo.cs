using System;
using System.Collections;
using JetBrains.Annotations;
using ONITwitchCore;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;

// ReSharper disable once CheckNamespace : must be kept for compatibility (TODO: maybe fix for 1.0?)
namespace EventLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
[PublicAPI]
public class EventInfo
{
	/// <summary>
	/// The full namespaced ID of the <see cref="EventInfo"/>.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string Id => $"{EventNamespace}.{EventId}";

	/// <summary>
	/// The friendly name of the <see cref="EventInfo"/>.
	/// </summary>
	[PublicAPI] [CanBeNull] public string FriendlyName;

	/// <summary>
	/// The namespace of the <see cref="EventInfo"/>.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string EventNamespace { get; }

	/// <summary>
	/// The ID of the <see cref="EventInfo"/>, without the <see cref="EventNamespace"/>.
	/// </summary>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public string EventId { get; }

	/// <summary>
	/// The <see cref="EventGroup"/> of the <see cref="EventInfo"/>.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public EventGroup Group { get; private set; }

	/// <summary>
	/// The <see cref="ONITwitchLib.Danger"/> of the <see cref="EventInfo"/>. 
	/// </summary>
	[PublicAPI] [CanBeNull] public Danger? Danger;

	/// <summary>
	/// Adds an <see cref="System.Action{T}"/> that is invoked with the event's data when the event is triggered.
	/// </summary>
	/// <param name="listener">The action to invoke when the event is triggered.</param>
	/// <seealso cref="DataManager"/>
	/// <seealso cref="Trigger"/>
	[PublicAPI]
	public void AddListener([NotNull] Action<object> listener)
	{
		actionRef.Action += listener;
	}

	/// <summary>
	/// Removes an <see cref="System.Action{T}"/> from the list of actions that are run when an event is triggered, if it exists. 
	/// </summary>
	/// <param name="listener">The action to remove.</param>
	/// <seealso cref="Trigger"/>
	[PublicAPI]
	public void RemoveListener([NotNull] Action<object> listener)
	{
		if (((IList) actionRef.Action.GetInvocationList()).Contains(listener))
		{
			actionRef.Action -= listener;
		}
	}

	/// <summary>
	/// Triggers the event with the specified data by calling each registered listener.
	/// Callers are expected to provide the correct type and values of data for this <see cref="EventInfo"/>.
	/// The correct data can typically be found in the <see cref="DataManager"/>.
	/// </summary>
	/// <param name="data">The data to call each listener with.</param>
	/// <seealso cref="AddListener"/>
	[PublicAPI]
	public void Trigger(object data)
	{
		try
		{
			actionRef.Action.Invoke(data);
		}
		catch (Exception e)
		{
			var debugName = FriendlyName != null ? $"{FriendlyName} ({Id})" : $"({Id})";
			Log.Warn($"crash while processing event {debugName}: {e}");
			DialogUtil.MakeDialog(
				ONITwitchCore.STRINGS.ONITWITCH.UI.DIALOGS.EVENT_ERROR.TITLE,
				string.Format(
					ONITwitchCore.STRINGS.ONITWITCH.UI.DIALOGS.EVENT_ERROR.BODY_FORMAT,
					debugName,
					e.Message
				),
				UI.CONFIRMDIALOG.OK,
				null
			);

			if ((Game.Instance != null) && Game.Instance.TryGetComponent(out VoteController controller))
			{
				controller.SetError();
			}
		}
	}

	/// <summary>
	/// Adds a condition to the event that should be run to determine if the event should run.
	/// </summary>
	/// <param name="condition">
	/// A function that takes an object parameter to be called with the event's data,
	/// and returns <see langword="true"/> if the event should be run and <see langword="false"/> if it should not. 
	/// </param>
	/// <seealso cref="Trigger"/>
	/// <seealso cref="DataManager"/>
	/// <seealso cref="CheckCondition"/>
	[PublicAPI]
	public void AddCondition([NotNull] Func<object, bool> condition)
	{
		if (conditionRef != null)
		{
			conditionRef.Condition += condition;
		}
		else
		{
			conditionRef = new ConditionRef(condition);
		}
	}

	/// <summary>
	/// Checks whether an event should be run by invoking each of its conditions and returning <see langword="false"/>
	/// if any of the conditions return <see langword="false"/>.
	/// </summary>
	/// <param name="data">The data to be passed to each condition.</param>
	/// <returns><see langword="false"/> if any of the conditions return false, otherwise <see langword="true"/>.</returns>
	/// <seealso cref="AddCondition"/>
	/// <seealso cref="Trigger"/>
	[PublicAPI]
	public bool CheckCondition(object data)
	{
		if (conditionRef != null)
		{
			foreach (var cond in conditionRef.Condition.GetInvocationList())
			{
				var result = (bool) cond.DynamicInvoke(data);
				if (!result)
				{
					return false;
				}
			}
		}

		// Either no condition or the conditions all passed
		return true;
	}

	private bool Equals(EventInfo other)
	{
		return (EventNamespace == other.EventNamespace) && (EventId == other.EventId);
	}

	/// <summary>
	/// Compares this <see cref="EventInfo"/> and another by their <see cref="Id"/>.
	/// </summary>
	/// <param name="obj">The other <see cref="EventInfo"/>.</param>
	/// <returns>Whether the two objects are equal.</returns>
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		return (obj.GetType() == GetType()) && Equals((EventInfo) obj);
	}

	/// <summary>
	/// Gets a hash code for an <see cref="EventInfo"/> based on its <see cref="Id"/>.
	/// </summary>
	/// <returns>A hash code for the object.</returns>
	public override int GetHashCode()
	{
		unchecked
		{
			return (EventNamespace.GetHashCode() * 397) ^ EventId.GetHashCode();
		}
	}

	/// <summary>
	/// Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	/// <seealso cref="FriendlyName"/>
	/// <seealso cref="Id"/>
	/// <seealso cref="EventNamespace"/>
	/// <seealso cref="EventId"/>
	public override string ToString()
	{
		return FriendlyName ?? Id;
	}

	/// <summary>
	/// this is dangerous to use, and not what you need
	/// </summary>
	internal void MoveToGroup(EventGroup newGroup, int weight)
	{
		Group.RemoveEvent(this);
		newGroup.AddEventInfoInternal(this, weight);
		Group = newGroup;
	}

	[NotNull] private readonly ActionRef actionRef = new(_ => { });
	[CanBeNull] private ConditionRef conditionRef;

	internal EventInfo(
		[NotNull] EventGroup group,
		[NotNull] string eventNamespace,
		[NotNull] string eventId,
		[CanBeNull] string friendlyName = null
	)
	{
		Group = group;
		EventNamespace = eventNamespace;
		EventId = eventId;
		FriendlyName = friendlyName;

		EventManager.Instance.RegisterEvent(this);
	}

	private class ActionRef
	{
		[NotNull] public Action<object> Action;

		public ActionRef([NotNull] Action<object> action)
		{
			Action = action;
		}
	}

	private class ConditionRef
	{
		[NotNull] public Func<object, bool> Condition;

		public ConditionRef([NotNull] Func<object, bool> condition)
		{
			Condition = condition;
		}
	}

	[Obsolete("Used as a cast helper for the reflection lib", true)]
	[CanBeNull]
	[UsedImplicitly]
	private int? GetDangerInt()
	{
		return Danger.HasValue ? (int) Danger.Value : null;
	}

	[Obsolete("Used as a cast helper for the reflection lib", true)]
	[UsedImplicitly]
	private void SetDangerInt([CanBeNull] int? danger)
	{
		Danger = danger.HasValue ? (Danger) danger.Value : null;
	}
}
