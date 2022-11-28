using ONITwitchCore.Toasts;
using ONITwitchLib;
using UnityEngine;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class ReduceOxygenCommand : CommandBase
{
	private static readonly CellModifyMassEvent ModifyEvent = new(
		"TwitchModifiedElement",
		"Modified by Twitch"
	);

	public override void Run(object data)
	{
		var targetFraction = (double) data;
		foreach (var cell in GridUtil.ActiveSimCells())
		{
			if (Grid.IsValidCell(cell) && (Grid.WorldIdx[cell] != byte.MaxValue) &&
				Grid.Element[cell].HasTag(GameTags.Breathable))
			{
				var mass = Grid.Mass[cell];
				// Only modify cells > 10g mass
				if (mass > 0.010f)
				{
					var massModification = (float) -(mass * (1 - targetFraction));
					SimMessages.ModifyMass(
						cell,
						massModification,
						byte.MaxValue,
						0,
						ModifyEvent,
						Grid.Temperature[cell],
						Grid.Element[cell].id
					);
				}
			}
		}

		ToastManager.InstantiateToast(
			"Oxygen Reduced",
			$"Breathable cells have been reduced by {Mathf.RoundToInt((float) ((1 - targetFraction) * 100))}% across the game"
		);
	}
}
