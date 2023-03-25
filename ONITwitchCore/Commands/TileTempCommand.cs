using ONITwitch.Content;
using ONITwitch.Toasts;
using UnityEngine;

namespace ONITwitch.Commands;

internal class TileTempCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return ComponentsExt.FloorTiles.Count > 0;
	}

	public override void Run(object data)
	{
		var tempMod = (float) (double) data;
		foreach (var tile in ComponentsExt.FloorTiles.Items)
		{
			var cell = Grid.PosToCell(tile);
			var newTemp = Mathf.Clamp(Grid.Temperature[cell] + tempMod, 1f, 9_999f);
			SimMessages.ModifyCell(
				cell,
				Grid.ElementIdx[cell],
				newTemp,
				Grid.Mass[cell],
				Grid.DiseaseIdx[cell],
				Grid.DiseaseCount[cell],
				SimMessages.ReplaceType.Replace
			);
		}

		if (tempMod > 0)
		{
			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.TILE_TEMP_UP.TITLE, STRINGS.ONITWITCH.TOASTS.TILE_TEMP_UP.BODY);
		}
		else
		{
			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.TILE_TEMP_DOWN.TITLE, STRINGS.ONITWITCH.TOASTS.TILE_TEMP_DOWN.BODY);
		}
	}
}
