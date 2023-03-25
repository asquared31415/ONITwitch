using System;
using System.Collections.Generic;
using System.Linq;
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

	private int selectedCell = Grid.InvalidCell;
	private bool debugClosestCell;

	private List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)> eventEntries;
	private string eventFilter = "";

	public TwitchDevTool()
	{
		Instance = this;
	}

	public void SelectedCell(int cell)
	{
		selectedCell = cell;
		foreach (var line in testingLines)
		{
			Object.Destroy(line);
		}

		testingLines.Clear();

		if (debugClosestCell)
		{
			var closest = GridUtil.NearestEmptyCell(selectedCell);
			if (closest != -1)
			{
				if (closest != selectedCell)
				{
					AddDebugMarker(selectedCell, Color.yellow with { a = 0.4f });
					AddDebugLine(selectedCell, closest, Color.green with { a = 0.4f });
				}

				AddDebugMarker(closest, Color.green with { a = 0.4f });
			}
			else
			{
				AddDebugMarker(selectedCell, Color.red with { a = 0.8f });
			}
		}
	}

	private static void SetStyle()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4);

		ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.06f, 0.06f, 0.06f, 0.75f));
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.09f, 0.00f, 0.21f, 1.00f));
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
		SetStyle();

		// ==========================================================
		// WARNING: game may not be active unless explicitly checked!
		// ==========================================================

		if (Game.Instance != null)
		{
			// ==========================================================
			// Game is active at this point
			// ==========================================================

			ImGui.Checkbox("Highlight nearest empty cell", ref debugClosestCell);

			ImGui.SliderFloat("Party Time Intensity", ref PartyTimePatch.Intensity, 0, 10);

			ImGui.Separator();
			ImGui.Text("Trigger Events");

			ImGui.Text("Events with a");
			ImGui.SameLine();
			ColoredBullet(ColorUtil.GreenSuccessColor);
			ImGui.SameLine();
			ImGui.Text("icon have their condition currently met. Events with a");
			ImGui.SameLine();
			ColoredBullet(ColorUtil.RedWarningColor);
			ImGui.SameLine();
			ImGui.Text("do not.");

			ImGui.Indent();

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
								eventInfo.Trigger(data);
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

	private readonly List<GameObject> testingLines = new();

	private void AddDebugMarker(int cell, Color color)
	{
		var go = new GameObject(TwitchModInfo.ModPrefix + "DebugLine");
		go.SetActive(true);
		var lineRenderer = go.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Klei/Biome/Unlit Transparent"))
		{
			renderQueue = RenderQueues.Liquid,
		};
		var pos = Grid.CellToPos(cell) with
		{
			z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2),
		};
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
		testingLines.Add(go);
	}

	private void AddDebugLine(int startCell, int endCell, Color color)
	{
		var gameObject = new GameObject(TwitchModInfo.ModPrefix + "DebugLine");
		gameObject.SetActive(true);
		var lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Klei/Biome/Unlit Transparent"))
		{
			renderQueue = RenderQueues.Liquid,
		};
		var startPos = Grid.CellToPosCCC(startCell, Grid.SceneLayer.FXFront2);
		var endPos = Grid.CellToPosCCC(endCell, Grid.SceneLayer.FXFront2);
		lineRenderer.SetPositions(new[] { startPos, endPos });
		lineRenderer.positionCount = 2;
		lineRenderer.startColor = lineRenderer.endColor = color;
		lineRenderer.startWidth = lineRenderer.endWidth = 0.05f;
		testingLines.Add(gameObject);
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
