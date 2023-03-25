using System.Linq;
using ONITwitch.Content;
using ONITwitchLib.Utils;
using ToastManager = ONITwitch.Toasts.ToastManager;

namespace ONITwitch.Commands;

internal class PoopsplosionCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);

	public override bool Condition(object data)
	{
		return ComponentsExt.Toilets.Count > 0;
	}

	public override void Run(object data)
	{
		const float pooMass = 1500f;
		var element = ElementLoader.FindElementByHash(SimHashes.ToxicSand);

		foreach (var baseCell in ComponentsExt.Toilets.Items.Select(Grid.PosToCell))
		{
			// base cell is always clear for all buildings, even if it were 1x1, it's that cell
			SimMessages.ReplaceAndDisplaceElement(
				baseCell,
				element.id,
				SpawnEvent,
				pooMass,
				element.defaultValues.temperature
			);

			foreach (var cell in GridUtil.GetNeighborsWithFoundationClearance(baseCell))
			{
				SimMessages.ReplaceAndDisplaceElement(
					cell,
					element.id,
					SpawnEvent,
					pooMass,
					element.defaultValues.temperature
				);
			}
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.POOPSPLOSION.TITLE, STRINGS.ONITWITCH.TOASTS.POOPSPLOSION.BODY);
	}
}
