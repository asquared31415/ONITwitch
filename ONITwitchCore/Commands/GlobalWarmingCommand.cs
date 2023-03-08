using System.Linq;
using ONITwitchCore.Toasts;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitchCore.Commands;

public class GlobalWarmingCommand : CommandBase
{
	public override void Run(object data)
	{
		// select a random world that is a root world
		var worlds = ClusterManager.Instance.WorldContainers.Where(
				world => world.IsDupeVisited && ((world.ParentWorldId == world.id) ||
												 (world.ParentWorldId == ClusterManager.INVALID_WORLD_IDX))
			)
			.ToList();
		if (worlds.Count == 0)
		{
			Log.Warn("Unable to find a suitable world for global warming");
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
				(Grid.IsGas(cell) || Grid.IsLiquid(cell) || Grid.Element[cell].HasTag(GameTags.IceOre)))
			{
				var element = Grid.Element[cell];
				var temp = Grid.Temperature[cell];

				const float transitionBuffer = 5;
				var targetTemp = Mathf.Clamp(
					temp + 20,
					element.lowTemp + transitionBuffer,
					element.highTemp - transitionBuffer
				);
				// should never be an issue as long as sim is in valid state and elements have normal values
				targetTemp = Mathf.Clamp(targetTemp, 0, 9_999);

				var shc = element.specificHeatCapacity;
				var mass = Grid.Mass[cell];
				var dTemp = targetTemp - Grid.Temperature[cell];

				var dQ = mass * shc * dTemp;

				SimMessages.ModifyEnergy(cell, dQ, 9_999f, SimMessages.EnergySourceID.DebugHeat);
			}
		}

		ToastManager
			.InstantiateToastWithPosTarget(
				STRINGS.TOASTS.GLOBAL_WARMING.TITLE,
				string.Format(
					Strings.Get(STRINGS.TOASTS.GLOBAL_WARMING.BODY_FORMAT.key),
					world.GetComponent<ClusterGridEntity>().Name
				),
				(Vector2) world.WorldOffset + world.WorldSize / 2
			);
	}
}
