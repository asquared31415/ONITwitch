namespace EventLib;

public class EventInfo
{
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

	public override string ToString()
	{
		return EventManager.Instance.GetFriendlyName(this) ?? Id;
	}
}
