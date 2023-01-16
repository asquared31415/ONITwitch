using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using JetBrains.Annotations;
using ONITwitchCore.Patches;
using ONITwitchLib;
using ONITwitchLib.Utils;
using UnityEngine;
using DataManager = EventLib.DataManager;
using EventInfo = EventLib.EventInfo;
using EventManager = EventLib.EventManager;
using Object = UnityEngine.Object;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore;

public class TwitchDevTool : DevTool
{
	public static TwitchDevTool Instance { get; private set; }

	private int selectedCell = Grid.InvalidCell;
	private bool debugClosestCell;

	private List<(string Namespace, List<EventInfo> Events)> eventEntries;
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

		// Everything below this needs the game to be active
		if (Game.Instance == null)
		{
			ImGui.TextColored(Color.red, "Game not yet active");
			return;
		}

		ImGui.Checkbox("Closest tile on click", ref debugClosestCell);

		ImGui.DragFloat("Party Time Intensity", ref PartyTimePatch.Intensity, 0.05f, 0, 10);

		ImGui.Separator();
		ImGui.Text("Trigger Events");
		ImGui.Indent();

		// initialize the entries with no filter
		eventEntries ??= GenerateEventEntries(null);

		if (ImGuiEx.InputFilter("Search", ref eventFilter, 100))
		{
			eventEntries = GenerateEventEntries(eventFilter);
		}

		var dataInst = DataManager.Instance;
		foreach (var (eventNamespace, eventInfos) in eventEntries)
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

				foreach (var eventInfo in eventInfos)
				{
					var buttonPressed = ImGui.Button($"{eventInfo}###{eventInfo.Id}");
					ImGuiEx.TooltipForPrevious($"ID: {eventInfo.Id}");
					if (buttonPressed)
					{
						Debug.Log($"[Twitch Integration] Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})");
						var data = dataInst.GetDataForEvent(eventInfo);
						eventInfo.Trigger(data);
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

	[MustUseReturnValue]
	[NotNull]
	private static List<(string Namespace, List<EventInfo> Events)> GenerateEventEntries([CanBeNull] string filter)
	{
		var namespacedEvents = new SortedDictionary<string, List<EventInfo>>();

		foreach (var eventInfo in EventManager.Instance.GetAllRegisteredEvents()
					 .Where(
						 info => string.IsNullOrWhiteSpace(filter) ||
								 (info.FriendlyName?.ToLowerInvariant().Contains(filter.ToLowerInvariant()) == true) ||
								 info.Id.ToLowerInvariant().Contains(filter.ToLowerInvariant())
					 ))
		{
			if (!namespacedEvents.ContainsKey(eventInfo.EventNamespace))
			{
				namespacedEvents.Add(eventInfo.EventNamespace, new List<EventInfo> { eventInfo });
			}
			else
			{
				namespacedEvents[eventInfo.EventNamespace].Add(eventInfo);
			}
		}

		foreach (var (_, events) in namespacedEvents)
		{
			events.Sort(
				(infoA, infoB) =>
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
			);
		}

		var filtered = new List<(string, List<EventInfo>)>();

		// put the base mod events in first, then the rest, sorted by namespace
		if (namespacedEvents.TryGetValue(TwitchModInfo.StaticID, out var baseEvents))
		{
			namespacedEvents.Remove(TwitchModInfo.StaticID);
			filtered.Add((TwitchModInfo.StaticID, baseEvents));
		}

		foreach (var (modNamespace, events) in namespacedEvents)
		{
			filtered.Add((modNamespace, events));
		}

		return filtered;
	}
}
