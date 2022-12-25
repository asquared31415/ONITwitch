using ONITwitchLib;
using ProcGen;
using EventManager = EventLib.EventManager;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class MeltMagmaCommand : CommandBase
{
	public override void Run(object data)
	{
		for (var cell = 0; cell < Grid.CellCount; cell++)
		{
			if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.MagmaCore)
			{
				if (!Grid.Foundation[cell])
				{
					var cellId = Grid.Element[cell].id;
					switch (cellId)
					{
						case SimHashes.IgneousRock or SimHashes.Vacuum or SimHashes.Magma:
							SimMessages.ReplaceElement(
								cell,
								SimHashes.Magma,
								null,
								1840,
								Constants.CELSIUS2KELVIN + 1727
							);
							break;
						case SimHashes.Obsidian:
							// heat up obsidian to the same temp too
							SimMessages.ReplaceElement(
								cell,
								SimHashes.Obsidian,
								null,
								Grid.Mass[cell],
								Constants.CELSIUS2KELVIN + 1727
							);
							break;
					}
				}
			}
		}

		var eventInfo = EventManager.Instance.GetEventByID(TwitchModInfo.StaticID, "MeltMagma")!;
		TwitchDeckManager.Instance.RemoveEvent(eventInfo);
		ToastManager.InstantiateToast("Magma Melted", "All magma in the core of the planet has been restored");
	}
}
