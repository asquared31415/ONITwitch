using System.Collections.Generic;
using ONITwitch.Content;
using ONITwitch.Toasts;
using UnityEngine;

namespace ONITwitch.Commands;

internal class UninsulateCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return ComponentsExt.InsulatedTiles.Count > 0;
	}

	private const float RemoveChance = 0.05f;

	public override void Run(object data)
	{
		var baseTile = Assets.GetBuildingDef(TileConfig.ID);
		foreach (var building in ComponentsExt.InsulatedTiles.Items)
		{
			if (Random.value <= RemoveChance)
			{
				var cell = Grid.PosToCell(building);
				if (building.TryGetComponent<Deconstructable>(out var deconstructable) &&
					building.TryGetComponent<SimCellOccupier>(out var sco) &&
					building.TryGetComponent<BuildingComplete>(out var buildingComplete) &&
					Grid.IsValidBuildingCell(cell) &&
					(Grid.Element[cell].id != SimHashes.Unobtanium))
				{
					if (Grid.Objects[cell, (int) baseTile.TileLayer] == building.gameObject)
					{
						Grid.Objects[cell, (int) baseTile.ObjectLayer] = null;
						Grid.Objects[cell, (int) baseTile.TileLayer] = null;
						Grid.Foundation[cell] = false;
					}

					sco.DestroySelf(
						() =>
						{
							foreach (var material in deconstructable.ForceDestroyAndGetMaterials())
							{
								Object.Destroy(material);
							}

							var buildMaterials = new List<Tag>
							{
								buildingComplete.primaryElement.ElementID.CreateTag(),
							};
							// use temp of building, but if it's too low, that means it was likely a building not yet set up   
							var temp = buildingComplete.primaryElement.Temperature;
							if (temp < 0.1)
							{
								temp = 20f + Constants.CELSIUS2KELVIN;
							}

							baseTile.Build(
								cell,
								Orientation.Neutral,
								null,
								buildMaterials,
								temp,
								timeBuilt: GameClock.Instance.GetTime()
							);
							TileVisualizer.RefreshCell(cell, baseTile.TileLayer, baseTile.ReplacementLayer);
						}
					);
				}
			}
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.UNINSULATE_TILES.TITLE, STRINGS.ONITWITCH.TOASTS.UNINSULATE_TILES.BODY);
	}
}
