using ONITwitchLib;
using ONITwitchLib.Utils;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

internal class ElementFloodCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);

	public override bool Condition(object data)
	{
		return ElementUtil.ElementExistsAndEnabled((string) data);
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

		var element = ElementUtil.FindElementByNameFast((string) data);
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

		ToastManager.InstantiateToastWithPosTarget(
			STRINGS.ONITWITCH.TOASTS.FLOOD.TITLE,
			string.Format(STRINGS.ONITWITCH.TOASTS.FLOOD.BODY_FORMAT, Util.StripTextFormatting(element.name)),
			Grid.CellToPos(nearestCell)
		);
	}
}
