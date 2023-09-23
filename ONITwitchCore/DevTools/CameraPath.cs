using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitch.DevTools;

internal readonly record struct CameraPathPoint(float OrthographicSize, Vector2 Position, float WaitTime)
{
	public override string ToString()
	{
		return $"{Position} Orthographic Size: {OrthographicSize} Delay {WaitTime}s";
	}
}

internal class CameraPath
{
	[NotNull] private readonly List<CameraPathPoint> camPoints = new();

	// The coroutine that is run to execute the camera path. Stored so that it can be canceled if needed.
	private Coroutine executeCamCoroutine;

	public IReadOnlyList<CameraPathPoint> CamPoints => new List<CameraPathPoint>(camPoints);

	public bool CanExecute()
	{
		return camPoints.Count > 0;
	}

	public void AddCameraCell(int cell)
	{
		Stop();

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

	public void Clear()
	{
		Stop();
		camPoints.Clear();
	}

	public void SetOrthographicSize(int idx, float size)
	{
		camPoints[idx] = camPoints[idx] with { OrthographicSize = size };
	}

	public void SetWaitTime(int idx, float time)
	{
		camPoints[idx] = camPoints[idx] with { WaitTime = time };
	}

	public void RemoveAt(int idx)
	{
		if (idx < camPoints.Count)
		{
			camPoints.RemoveAt(idx);
		}
	}

	// Makes the camera into cinematic mode, and then moves between the points specified in the cam points
	public void ExecuteCameraPath()
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

			if (camPoints.Count > 0)
			{
				var point = camPoints[0];
				Log.Debug($"Starting camera at {point}");
				CameraController.Instance.SnapTo(point.Position, point.OrthographicSize);
				yield return new WaitForSecondsRealtime(point.WaitTime);
				// ReSharper disable once ForCanBeConvertedToForeach
				// manual idx so that it can't cause a concurrent modification exception. now worst case it just is a little weird.
				for (var idx = 0; idx < camPoints.Count; idx++)
				{
					var camPoint = camPoints[idx];
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

			Log.Debug("Camera path complete");
			ResetCamState();
		}

		executeCamCoroutine = CameraController.Instance.StartCoroutine(Path());
	}

	private void Stop()
	{
		if (executeCamCoroutine != null)
		{
			CameraController.Instance.StopCoroutine(executeCamCoroutine);
			executeCamCoroutine = null;
			ResetCamState();
		}
	}

	private static void ResetCamState()
	{
		var trav = new Traverse(CameraController.Instance);
		trav.Field<bool>("cinemaCamEnabled").Value = false;
		DebugHandler.ToggleScreenshotMode();
		CameraController.Instance.SetWorldInteractive(true);
	}
}
