using JetBrains.Annotations;

namespace ONITwitch.Config;

internal class CommandConfig
{
	[CanBeNull] public string FriendlyName;
	[CanBeNull] public object Data;
	public int Weight;
	[CanBeNull] public string GroupName;
}
