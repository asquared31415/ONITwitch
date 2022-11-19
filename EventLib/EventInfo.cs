namespace EventLib;

public class EventInfo
{
	public string Id { get; }
	public string FriendlyName { get; }

	public EventInfo(string id)
	{
		Id = id;
	}

	public EventInfo(string id, string name)
	{
		Id = id;
		FriendlyName = name;
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
		return string.IsNullOrEmpty(FriendlyName) ? Id : FriendlyName;
	}
}
