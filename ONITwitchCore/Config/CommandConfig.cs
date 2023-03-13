using JetBrains.Annotations;

namespace ONITwitchCore.Config;

internal class CommandConfig
{
	[CanBeNull] public string FriendlyName;
	[CanBeNull] public object Data;
	public int Weight;
	[CanBeNull] public string GroupName;
}
