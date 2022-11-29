namespace ONITwitchLib;

/// <summary>
/// Represents an event that is known to the <see cref="EventManager"/>.
/// </summary>
public class EventInfo
{
	internal readonly object EventInfoInstance;

	internal EventInfo(object instance)
	{
		EventInfoInstance = instance;
	}

	/// <summary>
	/// Gets a string representation of the event.
	/// </summary>
	/// <returns>The friendly name of the event, if it exists, or the ID of the event otherwise.</returns>
	public override string ToString()
	{
		return EventInfoInstance.ToString();
	}
}
