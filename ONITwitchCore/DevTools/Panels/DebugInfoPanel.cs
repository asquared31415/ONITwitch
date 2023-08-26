using System;
using System.Linq;
using ImGuiNET;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.DevTools.Panels;

internal class DebugInfoPanel : IDevToolPanel
{
	[NotNull] private readonly DebugMarkers debugMarkers;

	// Whether to draw the closest cell to the selected cell.
	private bool debugClosestCell;

	public DebugInfoPanel([NotNull] DebugMarkers debugMarkers)
	{
		this.debugMarkers = debugMarkers;
	}

	public void DrawPanel()
	{
		DrawDebugClosestCell();

		if (ImGui.Button("Dump Valid Surprise Box Prefabs"))
		{
			Log.Debug("All valid prefabs for the Surprise Box:");
			foreach (var prefabID in Assets.Prefabs.Where(SurpriseBox.PrefabIsValid))
			{
				// Intentional use of Console.WriteLine to remove extra formatting for ease of data manipulation.
				Console.WriteLine(Util.StripTextFormatting(prefabID.name));
			}
		}
	}

	private void DrawDebugClosestCell()
	{
		ImGui.Checkbox("Highlight nearest empty cell", ref debugClosestCell);
		if (debugClosestCell)
		{
			if (SelectTool.Instance != null)
			{
				var startCell = SelectTool.Instance.GetSelectedCell();
				var closestCell = GridUtil.NearestEmptyCell(startCell);
				ImGui.Text($"Base Cell: {startCell}, Closest {closestCell}");

				if (Grid.IsValidCell(closestCell))
				{
					if (closestCell != startCell)
					{
						debugMarkers.AddCell(startCell, Color.yellow with { a = 0.4f });
						debugMarkers.AddLine(startCell, closestCell, Color.green with { a = 0.4f });
					}

					// Always draw the end cell.
					debugMarkers.AddCell(closestCell, Color.green with { a = 0.4f });
				}
				else
				{
					// Could not find a closest cell
					debugMarkers.AddCell(startCell, Color.red with { a = 0.8f });
				}
			}
			else
			{
				ImGui.TextColored(Color.red, "No SelectTool instance");
			}
		}
	}
}
