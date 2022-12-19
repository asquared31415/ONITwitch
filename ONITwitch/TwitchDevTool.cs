using System.Collections.Generic;
using EventLib;
using ImGuiNET;
using ONITwitchCore.Content.Buildings;
using ONITwitchLib.Utils;

namespace ONITwitch;

public class TwitchDevTool : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (Game.Instance == null)
		{
			ImGui.Text("Game not yet active");
			return;
		}

		if (ImGui.Button("Spawn Surprise Box"))
		{
			var spawnCell =
				GridUtil.FindCellOpenToBuilding(
					Grid.PosToCell(Components.LiveMinionIdentities.Items.GetRandom()),
					new[] { CellOffset.none }
				);
			var surpriseDef = Assets.GetBuildingDef(SurpriseBoxConfig.Id);
			surpriseDef.Build(
				spawnCell,
				Orientation.Neutral,
				null,
				new List<Tag> { SimHashes.SandStone.CreateTag() },
				273.15f,
				false
			);
		}

		ImGui.Separator();
		ImGui.Text("Trigger Events");
		ImGui.Indent();
		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;
		foreach (var eventInfo in eventInst.GetAllRegisteredEvents())
		{
			if (ImGui.Button(eventInfo.ToString()))
			{
				Debug.Log($"[Twitch Integration] Triggering Event {eventInfo} (id {eventInfo.Id})");
				var data = dataInst.GetDataForEvent(eventInfo);
				eventInst.TriggerEvent(eventInfo, data);
			}
		}
	}
}
