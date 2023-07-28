using System.Collections.Generic;
using System.Linq;
using Database;
using ONITwitch.Cmps;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitch.Content;

internal class ExtraStatusItems : StatusItems
{
	internal readonly StatusItem GeyserTemporarilyTuned;
	internal readonly StatusItem PoisonedStatusItem;

	internal ExtraStatusItems(ResourceSet parent) : base(TwitchModInfo.StaticID + nameof(ExtraStatusItems), parent)
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
			Log.Warn($"Data for poison tooltip was incorrect type: {ty}");
			return str;
		};

		GeyserTemporarilyTuned = Add(
			new StatusItem(
				TwitchModInfo.ModPrefix + "GeyserTemporarilyTuned",
				nameof(STRINGS.MISC),
				"",
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID
			)
		);
		GeyserTemporarilyTuned.resolveTooltipCallback = (str, o) =>
		{
			if (o is List<TimedModification> modifications)
			{
				var maxTime = modifications.Select(modification => modification.TimeRemaining)
					.Max();
				return str.Replace("{time}", GameUtil.GetFormattedCycles(maxTime));
			}

			var ty = o == null ? "null" : o.GetType().ToString();
			Log.Warn($"Data for geyser tune tooltip was incorrect type: {ty}");
			return str;
		};
	}
}
