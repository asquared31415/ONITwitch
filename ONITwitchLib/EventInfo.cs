namespace ONITwitchLib;

public class EventInfo
{
	internal readonly object EventInfoInstance;

	internal EventInfo(object instance)
	{
		EventInfoInstance = instance;
	}

	public override string ToString()
	{
		return EventInfoInstance.ToString();
	}
}
