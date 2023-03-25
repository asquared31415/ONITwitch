using ONITwitch.Cmps;
using ONITwitch.Patches;
using ONITwitch.Toasts;

namespace ONITwitch.Commands;

internal class PartyTimeCommand : CommandBase
{
	public override void Run(object data)
	{
		var time = (float) (double) data;
		var partyTime = Game.Instance.gameObject.AddOrGet<PartyTime>();
		partyTime.TimeRemaining = time;
		partyTime.enabled = true;
		PartyTimePatch.Enabled = true;
		
		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.PARTY_TIME.TITLE, STRINGS.ONITWITCH.TOASTS.PARTY_TIME.BODY);
	}
}
