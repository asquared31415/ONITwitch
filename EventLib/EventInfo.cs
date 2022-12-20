using System;

namespace EventLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
public class EventInfo : IComparable<EventInfo>, IComparable
{
	/// <summary>
	/// The ID of the event.
	/// </summary>
	public string Id => $"{eventNamespace}.{eventId}";

	public string Namespace => eventNamespace;
	public string EventId => eventId;

	private readonly string eventNamespace;
	private readonly string eventId;

	internal EventInfo(string eventNamespace, string eventId)
	{
		this.eventNamespace = eventNamespace;
		this.eventId = eventId;
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
		return EventManager.Instance.GetFriendlyName(this) ?? Id;
	}

	public int CompareTo(EventInfo other)
	{
		if (ReferenceEquals(this, other))
		{
			return 0;
		}

		if (ReferenceEquals(null, other))
		{
			return 1;
		}

		var namespaceComparison = string.Compare(eventNamespace, other.eventNamespace, StringComparison.Ordinal);
		return namespaceComparison != 0
			? namespaceComparison
			: string.Compare(eventId, other.eventId, StringComparison.Ordinal);
	}

	public int CompareTo(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return 1;
		}

		if (ReferenceEquals(this, obj))
		{
			return 0;
		}

		return obj is EventInfo other
			? CompareTo(other)
			: throw new ArgumentException($"Object must be of type {nameof(EventInfo)}");
	}
}
