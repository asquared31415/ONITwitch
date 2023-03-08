using System;
using System.Collections;
using JetBrains.Annotations;
using ONITwitchCore;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;

namespace EventLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
public class EventInfo
{
	/// <summary>
	/// The ID of the event.
	/// </summary>
	[NotNull]
	public string Id => $"{eventNamespace}.{eventId}";

	[CanBeNull] public string FriendlyName;

	[NotNull] public string EventNamespace => eventNamespace;
	[NotNull] public string EventId => eventId;

	[NotNull] public EventGroup Group { get; private set; }

	[CanBeNull] public Danger? Danger;

	[NotNull] private readonly string eventNamespace;
	[NotNull] private readonly string eventId;

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
		this.eventNamespace = eventNamespace;
		this.eventId = eventId;
		FriendlyName = friendlyName;

		EventManager.Instance.RegisterEvent(this);
	}

	/// <summary>
	/// this is dangerous to use, and probably not what you need
	/// </summary>
	public void MoveToGroup(EventGroup newGroup, int weight)
	{
		Group.RemoveEvent(this);
		newGroup.AddEventInfoInternal(this, weight);
		Group = newGroup;
	}

	public void AddListener([NotNull] Action<object> listener)
	{
		actionRef.Action += listener;
	}

	public void RemoveListener([NotNull] Action<object> listener)
	{
		if (!((IList) actionRef.Action.GetInvocationList()).Contains(listener))
		{
			throw new ArgumentException(
				$"unable to remove listener from event {Id}",
				nameof(listener)
			);
		}

		actionRef.Action -= listener;
	}

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

	protected bool Equals(EventInfo other)
	{
		return (eventNamespace == other.eventNamespace) && (eventId == other.eventId);
	}

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

		if (obj.GetType() != GetType())
		{
			return false;
		}

		return Equals((EventInfo) obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return ((eventNamespace != null ? eventNamespace.GetHashCode() : 0) * 397) ^
				   (eventId != null ? eventId.GetHashCode() : 0);
		}
	}

	/// <summary>
	/// Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	public override string ToString()
	{
		return FriendlyName ?? Id;
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
