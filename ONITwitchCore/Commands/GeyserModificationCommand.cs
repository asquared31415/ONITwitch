using System.Collections.Generic;
using System.Linq;
using ONITwitch.Content.Cmps;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ToastManager = ONITwitch.Toasts.ToastManager;

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

			// set an origin ID so that it doesn't crash
			modification.template.originID = TwitchModInfo.ModPrefix + nameof(GeyserModificationCommand);

			var tuning = target.gameObject.AddOrGet<TimedGeyserTuning>();

			var finalModification = modification.template;

			const int modificationStrengthMultiplier = 3;
			// note: the -1 is because it's adding to the original
			for (var i = 0; i < modificationStrengthMultiplier - 1; i++)
			{
				finalModification.AddValues(modification.template);
			}

			tuning.AddModification(25 * Constants.SECONDS_PER_CYCLE, finalModification);

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
					.Where(
						static geyser => !geyser.TryGetComponent(out Uncoverable uncoverable) || uncoverable.IsUncovered
					)
			);
		}

		return geysers;
	}
}
