using System.Collections.Generic;
using EventLib;
using ImGuiNET;
using ONITwitchCore.Toasts;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch;

public class TwitchDevTool : DevTool
{
	public static TwitchDevTool Instance { get; private set; }

	private int selectedCell = Grid.InvalidCell;
	private bool debugClosestCell;

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
			ImGui.Text("Game not yet active");
			return;
		}

		ImGui.Checkbox("Closest tile on click", ref debugClosestCell);

		ImGui.Separator();
		ImGui.Text("Trigger Events");
		ImGui.Indent();
		var eventInst = EventManager.Instance;
		var dataInst = DataManager.Instance;

		var namespacedEvents = new SortedDictionary<string, List<EventInfo>>();
		var eventKeys = eventInst.GetAllRegisteredEvents();
		foreach (var eventInfo in eventKeys)
		{
			if (!namespacedEvents.ContainsKey(eventInfo.Namespace))
			{
				namespacedEvents.Add(eventInfo.Namespace, new List<EventInfo> { eventInfo });
			}
			else
			{
				namespacedEvents[eventInfo.Namespace].Add(eventInfo);
			}
		}

		// sort events within their respective category
		foreach (var (_, events) in namespacedEvents)
		{
			events.Sort();
		}

		foreach (var (eventNamespace, eventInfos) in namespacedEvents)
		{
			var mod = Global.Instance.modManager.mods.Find(mod => mod.staticID == eventNamespace);
			var headerName = mod != null ? mod.title : eventNamespace;
			if (ImGui.CollapsingHeader(headerName))
			{
				ImGui.Indent();

				foreach (var eventInfo in eventInfos)
				{
					if (ImGui.Button($"{eventInfo} ({eventInfo.EventId})"))
					{
						Debug.Log($"[Twitch Integration] Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})");
						var data = dataInst.GetDataForEvent(eventInfo);
						eventInst.TriggerEvent(eventInfo, data);
					}
				}

				ImGui.Unindent();
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
}
