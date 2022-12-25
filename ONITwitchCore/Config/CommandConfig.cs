using JetBrains.Annotations;

namespace ONITwitchCore.Config;

public class CommandConfig
{
	[CanBeNull] public string FriendlyName;
	[CanBeNull] public object Data;
	public int Weight;
	[CanBeNull] public string GroupName;
}
