using Database;
using ONITwitch.Cmps;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitch.Content;

public class ExtraStatusItems : StatusItems
{
	public StatusItem PoisonedStatusItem;

	public ExtraStatusItems(ResourceSet parent) : base(TwitchModInfo.StaticID + nameof(ExtraStatusItems), parent)
	{
		PoisonedStatusItem = Add(
			new StatusItem(
				TwitchModInfo.ModPrefix + "Poisoned",
				nameof(STRINGS.MISC),
				"",
				StatusItem.IconType.Exclamation,
				NotificationType.DuplicantThreatening,
				false,
				OverlayModes.None.ID
			)
		);
		PoisonedStatusItem.resolveTooltipCallback = (str, o) =>
		{
			if (o is OniTwitchDamageOverTime dot)
			{
				return str.Replace("{time}", GameUtil.GetFormattedTime(dot.SecondsRemaining));
			}

			var ty = o == null ? "null" : o.GetType().ToString();
			Log.Warn($"Data was incorrect type: {ty}");
			return str;
		};
	}
}
