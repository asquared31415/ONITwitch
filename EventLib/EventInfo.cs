namespace EventLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
public class EventInfo
{
	/// <summary>
	/// The ID of the event.
	/// </summary>
	public string Id { get; }

	public EventInfo(string id)
	{
		Id = id;
	}

	protected bool Equals(EventInfo other)
	{
		return Id == other.Id;
	}

	public override bool Equals(object obj)
	{
		if (obj is EventInfo other)
		{
			return Equals(other);
		}

		return false;
	}

	public override int GetHashCode()
	{
		return Id != null ? Id.GetHashCode() : 0;
	}

	/// <summary>
	/// Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	public override string ToString()
	{
		return EventManager.Instance.GetFriendlyName(this) ?? Id;
	}
}
