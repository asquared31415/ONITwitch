using System.Collections.Generic;
using ImGuiNET;
using ONITwitchCore.Content.Buildings;
using ONITwitchLib.Utils;

namespace ONITwitch;

public class TwitchDevTool : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		ImGui.Text("Twitch Dev Tool");
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
	}
}
