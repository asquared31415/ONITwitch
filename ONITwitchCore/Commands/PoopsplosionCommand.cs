using System.Linq;
using ONITwitchCore.Content;
using ONITwitchLib;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class PoopsplosionCommand : CommandBase
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

			foreach (var cell in GridUtil.GetNeighborsWithBuildingClearance(baseCell))
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

		ToastManager.InstantiateToast("Poop Explosion", "All toilets have exploded with poop");
	}
}
