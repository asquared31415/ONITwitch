using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

namespace ONITwitch.DevTools.Panels;

internal class CameraPathPanel(DebugMarkers debugMarkers, CameraPath cameraPath) : IDevToolPanel
{
	// true if the path of the camera should be shown with debug markers.
	private bool showCameraPath;

	public void DrawPanel()
	{
		ImGui.Checkbox("Edit camera path", ref TwitchDevTool.Instance.EditingCamPath);
		ImGuiEx.TooltipForPrevious(
			"When this is enabled, click a tile in the world to set the camera's position to the middle of that tile"
		);

		ImGui.SameLine();

		ImGui.Checkbox("Show Path", ref showCameraPath);
		if (showCameraPath)
		{
			DebugCameraPath();
		}

		ImGuiEx.TooltipForPrevious("Show a debug overlay for the camera's path");
		ImGui.SameLine();

		// Push the button disabled and semi-transparent if the camera cannot execute its path.
		TwitchImGui.WithStyle(
			cameraPath.CanExecute()
				? null
				: new ImGuiStyle().AddStyle(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f),
			() =>
			{
				if (ImGuiInternal.ButtonEx(
						"Execute Path",
						cameraPath.CanExecute()
							? ImGuiInternal.ImGuiButtonFlags.None
							: ImGuiInternal.ImGuiButtonFlags.Internal_Disabled
					))
				{
					cameraPath.ExecuteCameraPath();
				}
			}
		);

		if (!cameraPath.CanExecute())
		{
			ImGuiEx.TooltipForPrevious("Create one or more points first");
		}

		ImGui.SameLine(0, 200);
		if (ImGui.Button("Clear##clearCamPoints"))
		{
			cameraPath.Clear();
		}

		ImGuiEx.TooltipForPrevious("Clears all points in the path");

		DrawCameraPointsTable();
	}

	private void DrawCameraPointsTable()
	{
		if (ImGui.BeginTable(
				"twitch_camera_points",
				4,
				ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchProp
			))
		{
			ImGui.TableSetupColumn("Position");
			ImGui.TableSetupColumn("Orthographic Size");
			ImGui.TableSetupColumn("Delay");
			ImGui.TableSetupColumn("Delete");
			ImGui.TableHeadersRow();

			var camPoints = cameraPath.CamPoints;

			var pointsToRemove = new List<int>(4);
			for (var idx = 0; idx < camPoints.Count; idx++)
			{
				var camPoint = camPoints[idx];
				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text(camPoint.Position.ToString());
				if (ImGui.IsItemClicked())
				{
					CameraController.Instance.SetTargetPos(camPoint.Position, camPoint.OrthographicSize, false);
				}

				ImGui.TableNextColumn();
				var size = camPoint.OrthographicSize;
				if (ImGui.InputFloat($"##size.{idx}", ref size, 0.1f, 1, "%.2f"))
				{
					cameraPath.SetOrthographicSize(idx, size);
				}

				ImGui.TableNextColumn();
				var delay = camPoint.WaitTime;
				if (ImGui.InputFloat($"##delay.{idx}", ref delay))
				{
					if (delay < 0)
					{
						delay = 0;
					}

					cameraPath.SetWaitTime(idx, delay);
				}

				ImGui.TableNextColumn();
				if (ImGui.Button($"Delete##removeCamPoint.{idx}"))
				{
					pointsToRemove.Add(idx);
				}
			}

			// Reverse so that indexes are in reverse order
			pointsToRemove.Reverse();
			foreach (var idx in pointsToRemove)
			{
				cameraPath.RemoveAt(idx);
			}

			ImGui.EndTable();
		}
	}

	private void DebugCameraPath()
	{
		var camPoints = cameraPath.CamPoints;
		// loop over pairs of points to draw lines
		for (var idx = 0; idx < camPoints.Count - 1; idx++)
		{
			var startPoint = camPoints[idx];
			var endPoint = camPoints[idx + 1];

			// Evenly distribute the colors over the hue
			var h = idx / (float) camPoints.Count;
			var color = Color.HSVToRGB(h, 1, 1);

			debugMarkers.AddLine(Grid.PosToCell(startPoint.Position), Grid.PosToCell(endPoint.Position), color, 0.15f);
		}
	}
}
