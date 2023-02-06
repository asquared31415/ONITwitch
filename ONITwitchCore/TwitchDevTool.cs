using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONITwitchCore.Config;
using ONITwitchCore.Patches;
using ONITwitchLib;
// using ONITwitchLib;
using ONITwitchLib.Utils;
using UnityEngine;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;
using Object = UnityEngine.Object;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore;

public class TwitchDevTool : DevTool
{
	public static TwitchDevTool Instance { get; private set; }

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

	protected override void RenderTo(DevPanel panel)
	{
		// WARNING: game may not be active unless explicitly checked!

		if (ImGui.Button("Test Toast"))
		{
			ToastManager.InstantiateToast(
				"Dev Testing Toast",
				"This is a testing toast.\n<color=#FF00FF>this is color</color> <i>this is italic</i> <b>this is bold</b>\n<link=\"eoautdhoetnauh\">testing link</link> aaaaaa"
			);
		}

		if (ImGui.Button("Dump Event Config"))
		{
			DumpCurrentConfig();
		}

		// Everything below this needs the game to be active
		if (Game.Instance == null)
		{
			ImGui.TextColored(Color.red, "Game not yet active");
			return;
		}

		ImGui.Checkbox("Highlight nearest empty cell", ref debugClosestCell);

		ImGui.DragFloat("Party Time Intensity", ref PartyTimePatch.Intensity, 0.05f, 0, 10);

		ImGui.Separator();
		ImGui.Text("Trigger Events");

		ImGui.Text("Events colored");
		ImGui.SameLine();
		ImGui.TextColored(ColorUtil.GreenSuccessColor, "green");
		ImGui.SameLine();
		ImGui.Text("have their condition currently met. Events colored");
		ImGui.SameLine();
		ImGui.TextColored(ColorUtil.RedWarningColor, "red");
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
						ImGui.PushStyleColor(ImGuiCol.Text, condColor);
						var buttonPressed = ImGui.Button($"{eventInfo}###{eventInfo.Id}");
						ImGui.PopStyleColor();

						ImGuiEx.TooltipForPrevious($"ID: {eventInfo.Id}");
						if (buttonPressed)
						{
							Debug.Log(
								$"[Twitch Integration] Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})"
							);
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

	private readonly List<GameObject> testingLines = new();

	private void AddDebugMarker(int cell, Color color)
	{
		var go = new GameObject("ONITwitch.DebugLine");
		go.SetActive(true);
		var lineRenderer = go.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Klei/Biome/Unlit Transparent"))
		{
			renderQueue = RenderQueues.Liquid,
		};
		var pos = Grid.CellToPos(cell) with
		{
			z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2)
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
		var gameObject = new GameObject("ONITwitch.DebugLine");
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

	public void DumpCurrentConfig()
	{
		var dataInst = DataManager.Instance;
		var deckInst = TwitchDeckManager.Instance;
		var data = new Dictionary<string, Dictionary<string, CommandConfig>>();

		foreach (var group in deckInst.GetGroups())
		{
			foreach (var (eventInfo, weight) in group.GetWeights())
			{
				var eventNamespace = eventInfo.EventNamespace;
				var eventId = eventInfo.EventId;

				var config = new CommandConfig
				{
					FriendlyName = eventInfo.FriendlyName,
					Data = dataInst.GetDataForEvent(eventInfo),
					Weight = weight,
					GroupName = group.Name,
				};
				if (data.TryGetValue(eventNamespace, out var namespaceEvents))
				{
					namespaceEvents[eventId] = config;
				}
				else
				{
					data[eventNamespace] = new Dictionary<string, CommandConfig> { [eventId] = config };
				}
			}
		}

		var ser = JsonConvert.SerializeObject(data, Formatting.None);
		Debug.Log(ser);
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
