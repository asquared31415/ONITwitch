using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ImGuiNET;
using JetBrains.Annotations;
using ONITwitch.Patches;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;
using DataManager = ONITwitch.EventLib.DataManager;
using EventInfo = ONITwitch.EventLib.EventInfo;
using Object = UnityEngine.Object;

namespace ONITwitch;

internal class TwitchDevTool : DevTool
{
	internal static TwitchDevTool Instance { get; private set; }

	// Flags for options that the user can toggle
	private bool cameraPathMode;
	private bool showCameraPath;
	private bool debugClosestCell;

	private bool useMouseRangeOverride;
	private float eventDelay;

	private List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)> eventEntries;
	private string eventFilter = "";

	public TwitchDevTool()
	{
		Instance = this;
	}

	public void SelectedCell(int cell)
	{
		// Add a new cell to the camera positions
		// Call only once, the camera positions store the cell
		if (cameraPathMode)
		{
			AddCameraCell(cell);
		}

		if (debugClosestCell)
		{
			AddClosestCellMarker(cell);
		}
	}

	private static void SetStyle()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4);

		ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.06f, 0.06f, 0.06f, 0.75f));
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.18f, 0.00f, 0.42f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.30f, 0.000f, 0.84f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.30f, 0.00f, 0.84f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(0.57f, 0.27f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.SliderGrab, new Vector4(0.60f, 0.33f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, new Vector4(0.71f, 0.40f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.57f, 0.27f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.64f, 0.34f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.71f, 0.40f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.57f, 0.27f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.64f, 0.34f, 1.00f, 1.00f));
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.71f, 0.40f, 1.00f, 1.00f));
	}

	private static void ClearStyle()
	{
		// these must match the counts pushed in SetStyle
		ImGui.PopStyleColor(11);
		ImGui.PopStyleVar(1);
	}

	protected override void RenderTo(DevPanel panel)
	{
		// clear the markers at the start of each frame
		foreach (var line in debugMarkers)
		{
			Object.Destroy(line);
		}

		debugMarkers.Clear();

		SetStyle();

		// ==========================================================
		// WARNING: game may not be active unless explicitly checked!
		// ==========================================================

		if (Game.Instance != null)
		{
			// ==========================================================
			// Game is active at this point
			// ==========================================================

			if (ImGui.CollapsingHeader("Camera Path"))
			{
				ImGui.Checkbox("Edit camera path", ref cameraPathMode);
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

				var isButtonDisabled = camPoints.Count == 0;
				if (isButtonDisabled)
				{
					ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
				}

				if (ImGuiInternal.ButtonEx(
						"Execute Path",
						isButtonDisabled
							? ImGuiInternal.ImGuiButtonFlags.Internal_Disabled
							: ImGuiInternal.ImGuiButtonFlags.None
					))
				{
					ExecuteCameraPath();
				}

				// pop the disabled settings if they were pushed before
				if (isButtonDisabled)
				{
					ImGui.PopStyleVar();
					ImGuiEx.TooltipForPrevious("Create one or more points first");
				}

				ImGui.SameLine(0, 200);
				if (ImGui.Button("Clear###clearCamPoints"))
				{
					camPoints.Clear();
				}

				ImGuiEx.TooltipForPrevious("Clears all points in the path");

				DrawCameraPointsTable();
			}

			ImGui.Separator();

			if (ImGui.CollapsingHeader("Debug Info"))
			{
				ImGui.Checkbox("Highlight nearest empty cell", ref debugClosestCell);
				if (debugClosestCell)
				{
					ImGui.Text($"Base Cell: {startCell}, Closest {closestCell}");
				}
			}

			if (debugClosestCell)
			{
				DrawDebugClosestCell();
			}

			ImGui.Separator();

			if (ImGui.CollapsingHeader("Events", ImGuiTreeNodeFlags.DefaultOpen))
			{
				// Indent everything in the header
				ImGui.Indent();

				ImGui.SliderFloat("Event Delay", ref eventDelay, 0, 60);

				ImGui.Checkbox("Override Mouse Range", ref useMouseRangeOverride);
				if (useMouseRangeOverride)
				{
					var range = PosUtil.MouseRangeOverride.HasValue ? PosUtil.MouseRangeOverride.Value : 5;
					ImGui.SliderInt("Range Override", ref range, 0, 20);
					PosUtil.MouseRangeOverride = range;
				}
				else
				{
					PosUtil.MouseRangeOverride = null;
				}

				ImGui.SliderFloat("Party Time Intensity", ref PartyTimePatch.Intensity, 0, 10);

				ImGui.NewLine();

				ImGui.Text("Events with a");
				ImGui.SameLine();
				ColoredBullet(ColorUtil.GreenSuccessColor);
				ImGui.SameLine();
				ImGui.Text("icon have their condition currently met. Events with a");
				ImGui.SameLine();
				ColoredBullet(ColorUtil.RedWarningColor);
				ImGui.SameLine();
				ImGui.Text("do not.");

				// initialize the entries with no filter
				eventEntries ??= GenerateEventEntries(null);

				if (ImGuiEx.InputFilter("Search###EventSearch", ref eventFilter, 100))
				{
					eventEntries = GenerateEventEntries(eventFilter);
				}

				var dataInst = DataManager.Instance;
				foreach (var (eventNamespace, groups) in eventEntries)
				{
					var mod = Global.Instance.modManager.mods.Find(mod => mod.staticID == eventNamespace);
					var headerName = Util.StripTextFormatting(mod != null ? mod.title : eventNamespace);
					var missingNamespace = headerName.IsNullOrWhiteSpace();
					if (missingNamespace)
					{
						ImGui.PushStyleColor(ImGuiCol.Text, Color.red);
						headerName = "MISSING NAMESPACE";
					}

					if (ImGui.CollapsingHeader(headerName))
					{
						ImGui.PushStyleColor(ImGuiCol.Text, Color.white);
						ImGui.Indent();

						var firstGroup = true;
						foreach (var (groupName, events) in groups)
						{
							if (firstGroup)
							{
								firstGroup = false;
							}
							else
							{
								ImGui.NewLine();
							}

							ImGui.Text(groupName);
							foreach (var eventInfo in events)
							{
								var condColor = eventInfo.CheckCondition(dataInst.GetDataForEvent(eventInfo))
									? ColorUtil.GreenSuccessColor
									: ColorUtil.RedWarningColor;
								ColoredBullet(condColor);
								var buttonPressed = ImGui.Button($"{eventInfo}###{eventInfo.Id}");

								ImGuiEx.TooltipForPrevious($"ID: {eventInfo.Id}");
								if (buttonPressed)
								{
									Log.Debug($"Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})");
									var data = dataInst.GetDataForEvent(eventInfo);
									GameScheduler.Instance.Schedule(
										"dev trigger event",
										eventDelay,
										_ => { eventInfo.Trigger(data); }
									);
								}
							}
						}

						ImGui.Unindent();
						ImGui.PopStyleColor();
					}

					if (missingNamespace)
					{
						// pop the red style if we pushed it before
						ImGui.PopStyleColor();
					}
				}

				// unindent for end of events header 
				ImGui.Unindent();
			}
		}
		else
		{
			ImGui.TextColored(Color.red, "Game not yet active");
		}

		ClearStyle();
	}

	private static void ColoredBullet(Color color)
	{
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.Bullet();
		ImGui.PopStyleColor();
	}

	private readonly List<GameObject> debugMarkers = new();

	private void AddDebugCellMarker(int cell, Color color)
	{
		var go = new GameObject(TwitchModInfo.ModPrefix + "DebugCellMarker");
		go.SetActive(true);
		var lineRenderer = go.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Klei/Biome/Unlit Transparent"))
		{
			renderQueue = RenderQueues.Liquid,
		};
		var pos = Grid.CellToPos(cell) with { z = Grid.GetLayerZ(Grid.SceneLayer.SceneMAX) };
		lineRenderer.SetPositions(
			new[]
			{
				pos + new Vector3(0.0f, 0.5f),
				pos + new Vector3(1f, 0.5f),
			}
		);
		lineRenderer.positionCount = 2;
		lineRenderer.startColor = lineRenderer.endColor = color;
		lineRenderer.startWidth = lineRenderer.endWidth = 1f;
		debugMarkers.Add(go);
	}

	private void AddDebugLine(int lineStartCell, int lineEndCell, Color color, float width = 0.05f)
	{
		var gameObject = new GameObject(TwitchModInfo.ModPrefix + "DebugLine");
		gameObject.SetActive(true);
		var lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"))
		{
			renderQueue = RenderQueues.Liquid,
		};
		var startPos = Grid.CellToPosCCC(lineStartCell, Grid.SceneLayer.SceneMAX);
		var endPos = Grid.CellToPosCCC(lineEndCell, Grid.SceneLayer.SceneMAX);
		lineRenderer.SetPositions(new[] { startPos, endPos });
		lineRenderer.positionCount = 2;
		lineRenderer.startColor = lineRenderer.endColor = color;
		lineRenderer.startWidth = lineRenderer.endWidth = width;
		debugMarkers.Add(gameObject);
	}

	private int startCell = Grid.InvalidCell;
	private int closestCell = Grid.InvalidCell;

	private void AddClosestCellMarker(int baseCell)
	{
		startCell = baseCell;
		closestCell = GridUtil.NearestEmptyCell(baseCell);
	}

	private void DrawDebugClosestCell()
	{
		if (closestCell != -1)
		{
			if (closestCell != startCell)
			{
				AddDebugCellMarker(startCell, Color.yellow with { a = 0.4f });
				AddDebugLine(startCell, closestCell, Color.green with { a = 0.4f });
			}

			AddDebugCellMarker(closestCell, Color.green with { a = 0.4f });
		}
		else
		{
			// Could not find a closest cell
			AddDebugCellMarker(startCell, Color.red with { a = 0.8f });
		}
	}

	[NotNull] private readonly List<CameraPathPoint> camPoints = new();

	private struct CameraPathPoint
	{
		internal Vector2 Position;
		internal float OrthographicSize;
		internal float WaitTime;

		public override string ToString()
		{
			return $"{Position} Orthographic Size: {OrthographicSize} Delay {WaitTime}s";
		}
	}

	private void AddCameraCell(int cell)
	{
		// layer does not matter, the camera is at one position
		var pos = (Vector2) Grid.CellToPosCCC(cell, Grid.SceneLayer.Front);
		var point = new CameraPathPoint
		{
			Position = pos,
			OrthographicSize = CameraController.Instance.OrthographicSize,
			WaitTime = 2,
		};
		Log.Debug($"adding camera path {point}");
		camPoints.Add(point);
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
				if (ImGui.InputFloat($"###size.{idx}", ref size, 0.1f, 1, "%.2f"))
				{
					camPoints[idx] = camPoint with { OrthographicSize = size };
				}

				ImGui.TableNextColumn();
				var delay = camPoint.WaitTime;
				if (ImGui.InputFloat($"###delay.{idx}", ref delay))
				{
					if (delay < 0)
					{
						delay = 0;
					}

					camPoints[idx] = camPoint with { WaitTime = delay };
				}

				ImGui.TableNextColumn();
				if (ImGui.Button($"Delete###removeCamPoint.{idx}"))
				{
					pointsToRemove.Add(idx);
				}
			}

			// Reverse so that indexes are in reverse order
			pointsToRemove.Reverse();
			foreach (var idx in pointsToRemove)
			{
				camPoints.RemoveAt(idx);
			}

			ImGui.EndTable();
		}
	}

	private void DebugCameraPath()
	{
		// loop over pairs of points to draw lines
		for (var idx = 0; idx < camPoints.Count - 1; idx++)
		{
			var startPoint = camPoints[idx];
			var endPoint = camPoints[idx + 1];

			// Evenly distribute the colors over the hue
			var h = idx / (float) camPoints.Count;
			var color = Color.HSVToRGB(h, 1, 1);

			AddDebugLine(Grid.PosToCell(startPoint.Position), Grid.PosToCell(endPoint.Position), color, 0.15f);
		}
	}

	// Makes the camera into cinematic mode, and then moves between the points specified in the cam points
	private void ExecuteCameraPath()
	{
		Log.Debug("Executing camera path");

		IEnumerator Path()
		{
			CameraController.Instance.SetWorldInteractive(false);

			// Manually disable screenshot mode so that it can be re-enabled properly
			DebugHandler.ScreenshotMode = false;
			DebugHandler.ToggleScreenshotMode();

			var trav = new Traverse(CameraController.Instance);
			trav.Field<bool>("cinemaCamEnabled").Value = true;
			ManagementMenu.Instance.CloseAll();

			using (var points = camPoints.GetEnumerator())
			{
				if (points.MoveNext())
				{
					Log.Debug($"Starting camera at {points.Current}");
					CameraController.Instance.SnapTo(points.Current.Position, points.Current.OrthographicSize);
					yield return new WaitForSecondsRealtime(points.Current.WaitTime);
					while (points.MoveNext())
					{
						var camPoint = points.Current;
						Log.Debug($"Moving to {camPoint}");
						CameraController.Instance.SetTargetPos(camPoint.Position, camPoint.OrthographicSize, false);
						yield return new WaitUntil(
							() => ((Vector2) CameraController.Instance.transform.position - camPoint.Position)
								  .magnitude <
								  0.001
						);
						yield return new WaitForSecondsRealtime(camPoint.WaitTime);
					}
				}
			}

			Log.Debug("Camera path complete");

			trav.Field<bool>("cinemaCamEnabled").Value = false;
			DebugHandler.ToggleScreenshotMode();
			CameraController.Instance.SetWorldInteractive(true);
		}

		CameraController.Instance.StartCoroutine(Path());
	}

	[MustUseReturnValue]
	[NotNull]
	private static List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)>
		GenerateEventEntries([CanBeNull] string filter)
	{
		bool MatchesFilter([NotNull] EventInfo info)
		{
			return string.IsNullOrWhiteSpace(filter) ||
				   (info.FriendlyName?.ToLowerInvariant().Contains(filter.ToLowerInvariant()) == true) ||
				   info.Id.ToLowerInvariant().Contains(filter.ToLowerInvariant());
		}

		var namespacedGroupedEvents = new Dictionary<string, Dictionary<string, List<EventInfo>>>();
		foreach (var eventGroup in TwitchDeckManager.Instance.GetGroups())
		{
			foreach (var (info, _) in eventGroup.GetWeights())
			{
				if (MatchesFilter(info))
				{
					var eventNamespace = info.EventNamespace;
					if (!namespacedGroupedEvents.ContainsKey(eventNamespace))
					{
						namespacedGroupedEvents[eventNamespace] = new Dictionary<string, List<EventInfo>>();
					}

					var groupName = info.Group.Name.Contains("__<nogroup>__") ? "__NoGroup" : info.Group.Name;
					if (!namespacedGroupedEvents[eventNamespace].ContainsKey(groupName))
					{
						namespacedGroupedEvents[eventNamespace][groupName] = new List<EventInfo>();
					}

					namespacedGroupedEvents[eventNamespace][groupName].Add(info);
				}
			}
		}

		// sort events by name and then ID
		int CompareInfo(EventInfo infoA, EventInfo infoB)
		{
			// if both have a friendly name, use that
			var nameA = infoA.FriendlyName;
			var nameB = infoB.FriendlyName;
			if ((nameA != null) && (nameB != null))
			{
				return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
			}

			// if neither have a friendly name, then compare IDs
			if ((nameA == null) && (nameB == null))
			{
				return string.Compare(infoA.Id, infoB.Id, StringComparison.OrdinalIgnoreCase);
			}

			// the event without a name is greater (last) 
			return nameA == null ? 1 : -1;
		}

		List<(string, List<EventInfo>)> SortGroupsForNamespace(Dictionary<string, List<EventInfo>> groups)
		{
			var sorted = groups.OrderBy(entry => entry.Key).Select(pair => (pair.Key, pair.Value)).ToList();
			foreach (var (_, events) in sorted)
			{
				events.Sort(CompareInfo);
			}

			return sorted;
		}

		var filtered = new List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)>();

		// put the base mod events in first, then the rest, sorted by namespace
		if (namespacedGroupedEvents.TryGetValue(TwitchModInfo.StaticID, out var baseGroups))
		{
			namespacedGroupedEvents.Remove(TwitchModInfo.StaticID);
			filtered.Add((TwitchModInfo.StaticID, SortGroupsForNamespace(baseGroups)));
		}

		foreach (var (modNamespace, groups) in namespacedGroupedEvents)
		{
			filtered.Add((modNamespace, SortGroupsForNamespace(groups)));
		}

		return filtered;
	}
}
