using ONITwitchCore.Cmps;
using ONITwitchCore.Patches;

namespace ONITwitchCore.Commands;

public class PartyTimeCommand : CommandBase
{
	public override void Run(object data)
	{
		var time = (float) (double) data;
		var partyTime = Game.Instance.gameObject.AddOrGet<PartyTime>();
		partyTime.TimeRemaining = time;
		partyTime.enabled = true;
		PartyTimePatch.Enabled = true;
	}
}
