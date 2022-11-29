using System.Linq;
using ONITwitchLib;
using UnityEngine;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class IceAgeCommand : CommandBase
{
	// prevent these liquids from freezing
	// Magma: freezes the core too quickly and irreparably
	private static readonly SimHashes[] ForbiddenLiquids = { SimHashes.Magma };

	public override void Run(object data)
	{
		foreach (var cell in GridUtil.ActiveSimCells())
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

		ToastManager.InstantiateToast("Ice Age", "All liquids in the game have frozen");
	}
}
