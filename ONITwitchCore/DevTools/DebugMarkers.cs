using System.Collections.Generic;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.DevTools;

internal class DebugMarkers
{
	private readonly List<GameObject> debugMarkers = new();

	/// <summary>
	///     Clears all markers. Should be called at the beginning of every update.
	/// </summary>
	internal void Clear()
	{
		foreach (var line in debugMarkers)
		{
			Object.Destroy(line);
		}

		debugMarkers.Clear();
	}

	internal void AddCell(int cell, Color color)
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

	internal void AddLine(int lineStartCell, int lineEndCell, Color color, float width = 0.05f)
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
}
