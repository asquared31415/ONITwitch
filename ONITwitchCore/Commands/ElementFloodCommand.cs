using ONITwitchLib;

namespace ONITwitchCore.Commands;

public class ElementFloodCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);
	
	private Element element;
	
	public override bool Condition(object data)
	{
		element ??= ElementUtil.FindElementByNameFast((string) data);

		return ElementUtil.ElementExistsAndEnabled(element);
	}

	public override void Run(object data)
	{
		var cell = PosUtil.RandomCellNearMouse();
		var nearestCell = GridUtil.FindCellWithCavityClearance(cell);

		const int maxFloodSize = 41; // A diamond with 4 cells from the middle filled in
		var cells = GridUtil.FloodCollectCells(
			nearestCell,
			c => (int) (Grid.BuildMasks[c] & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation)) == 0,
			maxFloodSize
		);

		// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
		var mass = element.id switch
		{
			SimHashes.Magma => 200f,
			SimHashes.MoltenGold => 500f,
			_ => element.defaultValues.mass * 2f,
		};

		foreach (var i in cells)
		{
			SimMessages.ReplaceAndDisplaceElement(
				i,
				element.id,
				SpawnEvent,
				mass,
				element.defaultValues.temperature
			);
		}

		/*
		ToastUiManager.InstantiateToastWithPosTarget(
			"Flood",
			$"A flood of {Util.StripTextFormatting(element.name)} has spawned",
			Grid.CellToPos(nearestCell)
		);
		*/
	}
}
