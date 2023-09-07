using ImGuiNET;
using JetBrains.Annotations;
using ONITwitch.DevTools.Panels;
using UnityEngine;

namespace ONITwitch.DevTools;

internal class TwitchDevTool : DevTool
{
	// Panels for the dev tools.
	[NotNull] private readonly CameraPath cameraPath;
	[NotNull] private readonly CameraPathPanel cameraPathPanel;
	[NotNull] private readonly DebugInfoPanel debugInfoPanel;

	// Handles creating debug markers. In the future this will use pooled objects.
	[NotNull] private readonly DebugMarkers debugMarkers;
	[NotNull] private readonly EventsPanel eventsPanel;

	// The primary style used by the dev tools.
	[NotNull] private readonly ImGuiStyle mainStyle;

	// true if the camera path is currently being edited.
	internal bool EditingCamPath;

	public TwitchDevTool()
	{
		Instance = this;

		debugMarkers = new DebugMarkers();
		cameraPath = new CameraPath();
		cameraPathPanel = new CameraPathPanel(debugMarkers, cameraPath);
		debugInfoPanel = new DebugInfoPanel(debugMarkers);
		eventsPanel = new EventsPanel();

		mainStyle = new ImGuiStyle();
		mainStyle.AddStyle(ImGuiStyleVar.FrameRounding, 4);

		mainStyle.AddColor(ImGuiCol.WindowBg, new Color(0.06f, 0.06f, 0.06f, 0.75f));
		mainStyle.AddColor(ImGuiCol.FrameBg, new Color(0.18f, 0.00f, 0.42f, 1.00f));
		mainStyle.AddColor(ImGuiCol.FrameBgActive, new Color(0.30f, 0.000f, 0.84f, 1.00f));
		mainStyle.AddColor(ImGuiCol.FrameBgHovered, new Color(0.30f, 0.00f, 0.84f, 1.00f));
		mainStyle.AddColor(ImGuiCol.CheckMark, new Color(0.57f, 0.27f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.SliderGrab, new Color(0.60f, 0.33f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.SliderGrabActive, new Color(0.71f, 0.40f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.Button, new Color(0.57f, 0.27f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.ButtonHovered, new Color(0.64f, 0.34f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.ButtonActive, new Color(0.71f, 0.40f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.Header, new Color(0.57f, 0.27f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.HeaderHovered, new Color(0.64f, 0.34f, 1.00f, 1.00f));
		mainStyle.AddColor(ImGuiCol.HeaderActive, new Color(0.71f, 0.40f, 1.00f, 1.00f));
	}

	internal static TwitchDevTool Instance { get; private set; }

	public void SelectedCell(int cell)
	{
		// Add a new cell to the camera positions
		// Call only once, the camera positions store the cell
		if (EditingCamPath)
		{
			cameraPath.AddCameraCell(cell);
		}
	}

	protected override void RenderTo(DevPanel panel)
	{
		// ==========================================================
		// WARNING: game may not be active unless explicitly checked!
		// ==========================================================

		// Clear the markers at the start of each frame.
		debugMarkers.Clear();

		// Early out if the game isn't active, we can't do most things.
		if (Game.Instance == null)
		{
			TwitchImGui.WithStyle(mainStyle, () => { ImGui.TextColored(Color.red, "Game not yet active"); });
			return;
		}

		// ==========================================================
		// Game is active at this point
		// ==========================================================

		TwitchImGui.WithStyle(
			mainStyle,
			() =>
			{
				if (ImGui.CollapsingHeader("Debug Info", ImGuiTreeNodeFlags.DefaultOpen))
				{
					debugInfoPanel.DrawPanel();
				}

				ImGui.Separator();

				if (ImGui.CollapsingHeader("Camera Path"))
				{
					cameraPathPanel.DrawPanel();
				}

				ImGui.Separator();

				if (ImGui.CollapsingHeader("Events", ImGuiTreeNodeFlags.DefaultOpen))
				{
					eventsPanel.DrawPanel();
				}
			}
		);
	}
}
