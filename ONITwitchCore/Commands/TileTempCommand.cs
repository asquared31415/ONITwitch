using ONITwitchCore.Content;
using ONITwitchCore.Toasts;
using UnityEngine;

namespace ONITwitchCore.Commands;

public class TileTempCommand : CommandBase
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
			ToastManager.InstantiateToast(STRINGS.TOASTS.TILE_TEMP_UP.TITLE, STRINGS.TOASTS.TILE_TEMP_UP.BODY);
		}
		else
		{
			ToastManager.InstantiateToast(STRINGS.TOASTS.TILE_TEMP_DOWN.TITLE, STRINGS.TOASTS.TILE_TEMP_DOWN.BODY);
		}
	}
}
