using System.Collections.Generic;
using System.Linq;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using ToastManager = ONITwitch.Toasts.ToastManager;

namespace ONITwitch.Commands;

internal class FillBedroomCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);

	public override bool Condition(object data)
	{
		return (GetValidRooms().Count > 0) && ElementUtil.ElementExistsAndEnabled((string) data);
	}

	public override void Run(object data)
	{
		var element = ElementUtil.FindElementByNameFast((string) data);
		if (!ElementUtil.ElementExistsAndEnabled(element))
		{
			Log.Warn($"Unable to spawn element {(string) data}");
			return;
		}

		foreach (var bedroom in GetValidRooms())
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
			STRINGS.ONITWITCH.TOASTS.FILL_BEDROOMS.TITLE,
			string.Format(
				STRINGS.ONITWITCH.TOASTS.FILL_BEDROOMS.BODY_FORMAT,
				Util.StripTextFormatting(element.name)
			)
		);
	}

	private static List<Room> GetValidRooms()
	{
		var db = Db.Get();
		var bedroomType = db.RoomTypes.Bedroom;
		var barracksType = db.RoomTypes.Barracks;
		var privBedroomType = db.RoomTypes.PrivateBedroom;
		return Game.Instance.roomProber.rooms.Where(
				room => (room.roomType == bedroomType) ||
						(room.roomType == barracksType) ||
						(room.roomType == privBedroomType)
			)
			.ToList();
	}
}
