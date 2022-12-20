using System.Collections.Generic;
using EventLib;
using ImGuiNET;
using ONITwitchCore.Content.Buildings;
using ONITwitchCore.Toasts;
using ONITwitchLib.Utils;

namespace ONITwitch;

public class TwitchDevTool : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		// WARNING: game may not be active in this method unless explicitly checked!
		if (ImGui.Button("Test Toast"))
		{
			ToastManager.InstantiateToast(
				"Dev Testing Toast",
				"This is a testing toast created by the Twitch Integration dev tools."
			);
		}

		// Everything below this needs the game to be active
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
		var eventKeys = eventInst.GetAllRegisteredEvents();
		eventKeys.Sort();
		foreach (var eventInfo in eventKeys)
		{
			if (ImGui.Button($"{eventInfo} ({eventInfo.Id})"))
			{
				Debug.Log($"[Twitch Integration] Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})");
				var data = dataInst.GetDataForEvent(eventInfo);
				eventInst.TriggerEvent(eventInfo, data);
			}
		}
	}
}
