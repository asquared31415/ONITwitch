using System.Linq;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.Commands;

internal class IceAgeCommand : CommandBase
{
	// prevent these liquids from freezing
	// Magma: freezes the core too quickly and irreparably
	private static readonly SimHashes[] ForbiddenLiquids = { SimHashes.Magma };

	public override void Run(object data)
	{
		// select a random world that is a root world
		var worlds = ClusterManager.Instance.WorldContainers.Where(
				static world => world.IsDupeVisited && ((world.ParentWorldId == world.id) ||
														(world.ParentWorldId == ClusterManager.INVALID_WORLD_IDX))
			)
			.ToList();
		if (worlds.Count == 0)
		{
			Log.Warn("Unable to find a suitable world for Ice Age");
			foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
			{
				Log.Debug(
					$"{worldContainer.GetComponent<ClusterGridEntity>().Name}(id {worldContainer.id}) has parent {worldContainer.ParentWorldId}"
				);
			}

			return;
		}

		var world = worlds.GetRandom();
		foreach (var cell in GridUtil.IterateCellRegion(world.WorldOffset, world.WorldOffset + world.WorldSize))
		{
			if (Grid.IsWorldValidCell(cell) &&
				Grid.Element[cell].HasTag(GameTags.Liquid) &&
				!ForbiddenLiquids.Contains(Grid.Element[cell].id))
			{
				var element = Grid.Element[cell];
				var targetTemp = Mathf.Clamp(element.lowTemp - 6f, 0f, 9_999f);
				var temp = Grid.Temperature[cell];

				if (temp < targetTemp)
				{
					continue;
				}

				var shc = element.specificHeatCapacity;
				var mass = Grid.Mass[cell];
				var dTemp = targetTemp - Grid.Temperature[cell];

				var dQ = mass * shc * dTemp;

				SimMessages.ModifyEnergy(cell, dQ, 9_999f, SimMessages.EnergySourceID.DebugCool);
			}
		}

		ToastManager
			.InstantiateToastWithPosTarget(
				STRINGS.ONITWITCH.TOASTS.ICE_AGE.TITLE,
				string.Format(
					STRINGS.ONITWITCH.TOASTS.ICE_AGE.BODY_FORMAT,
					world.GetComponent<ClusterGridEntity>().Name
				),
				(Vector2) world.WorldOffset + world.WorldSize / 2
			);
	}
}
