using System.Collections.Generic;
using System.Linq;
using ONITwitch.Cmps;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;

namespace ONITwitch.Commands;

internal class GeyserModificationCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return RevealedGeysers().Count > 0;
	}

	public override void Run(object data)
	{
		var geysers = RevealedGeysers();
		if (geysers.Count > 0)
		{
			var target = geysers.GetRandom();
			if (!GeoTunerConfig.geotunerGeyserSettings.TryGetValue(target.configuration.typeId, out var modification))
			{
				Log.Warn($"No modification registered for geyser type {target.configuration.typeId}, using default");
				modification = GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.DEFAULT_CATEGORY];
			}

			var tuning = target.gameObject.AddOrGet<TimedGeyserTuning>();
			tuning.AddModification(25 * Constants.SECONDS_PER_CYCLE, modification.template);

			ToastManager.InstantiateToastWithGoTarget(
				STRINGS.ONITWITCH.TOASTS.GEYSER_MODIFICATION.TITLE,
				string.Format(
					STRINGS.ONITWITCH.TOASTS.GEYSER_MODIFICATION.BODY_FORMAT,
					target.GetComponent<UserNameable>().savedName
				),
				target.gameObject
			);
		}
		else
		{
			Log.Warn("Unable to find any geysers to modify");
		}
	}

	private static List<Geyser> RevealedGeysers()
	{
		var geysers = new List<Geyser>();
		foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
		{
			var idx = worldContainer.id;
			geysers.AddRange(
				Components.Geysers.GetItems(idx)
					.Where(geyser => !geyser.TryGetComponent(out Uncoverable uncoverable) || uncoverable.IsUncovered)
			);
		}

		return geysers;
	}
}
