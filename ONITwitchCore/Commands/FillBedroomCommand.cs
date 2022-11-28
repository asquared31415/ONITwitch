using System.Linq;
using ONITwitchCore.Toasts;
using ONITwitchLib;

namespace ONITwitchCore.Commands;

public class FillBedroomCommand : CommandBase
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
		var element = ElementUtil.FindElementByNameFast((string) data);
		if (!ElementUtil.ElementExistsAndEnabled(element))
		{
			Debug.LogWarning($"[Twitch Integration] Unable to spawn element {(string) data}");
			return;
		}

		var db = Db.Get();
		var bedroomType = db.RoomTypes.Bedroom;
		var barracksType = db.RoomTypes.Barracks;
		foreach (var bedroom in Game.Instance.roomProber.rooms.Where(
					 room => (room.roomType == bedroomType) || (room.roomType == barracksType)
				 ))
		{
			foreach (var bed in bedroom.buildings.Where(building => building.GetComponent<Bed>() != null))
			{
				var cell = Grid.PosToCell(bed);
				if (Grid.IsValidCell(cell))
				{
					SimMessages.ReplaceAndDisplaceElement(
						cell,
						element.id,
						SpawnEvent,
						500f,
						element.defaultValues.temperature
					);
				}
			}
		}

		ToastManager.InstantiateToast(
			"Bedrooms Bombed",
			$"Every bedroom has had {Util.StripTextFormatting(element.name)} created inside it"
		);
	}
}
