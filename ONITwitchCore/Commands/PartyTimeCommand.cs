using ONITwitch.Content.Cmps;
using ONITwitch.Patches;
using ONITwitch.Settings.Components;
using ONITwitch.Toasts;

namespace ONITwitch.Commands;

internal class PartyTimeCommand : CommandBase
{
	public override bool Condition(object data)
	{
		// If photosensitive mode is enabled, this event must not appear 
		return !TwitchSettings.GetConfig().PhotosensitiveMode;
	}

	public override void Run(object data)
	{
		var time = (float) (double) data;
		var partyTime = Game.Instance.gameObject.AddOrGet<OniTwitchPartyTime>();
		partyTime.TimeRemaining = time;
		partyTime.enabled = true;
		PartyTimePatch.Enabled = true;

		ToastManager.InstantiateToast(
			STRINGS.ONITWITCH.TOASTS.PARTY_TIME.TITLE,
			STRINGS.ONITWITCH.TOASTS.PARTY_TIME.BODY
		);
	}
}
