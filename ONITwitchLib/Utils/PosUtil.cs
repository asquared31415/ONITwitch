using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib.Utils;

/// <summary>
/// Utilities for getting relevant positions for events to use.
/// </summary>
[PublicAPI]
public static class PosUtil
{
	/// <summary>
	/// Gets the world position of the mouse within the bounds of a world and the screen.
	/// </summary>
	/// <returns>The world position of the mouse, within the current active world and clamped to the screen boundaries.</returns>
	[PublicAPI]
	public static Vector3 ClampedMouseWorldPos()
	{
		if (Camera.main != null)
		{
			var worldPoint = Camera.main.ScreenToWorldPoint(ClampedMousePos());
			var currentWorldMin = ClusterManager.Instance.activeWorld.minimumBounds;
			var currentWorldMax = ClusterManager.Instance.activeWorld.maximumBounds;
			var clamped = new Vector3(
				Mathf.Clamp(worldPoint.x, currentWorldMin.x, currentWorldMax.x),
				Mathf.Clamp(worldPoint.y, currentWorldMin.y, currentWorldMax.y),
				worldPoint.z
			);
			return clamped;
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Gets the <see cref="Grid"/> cell that the mouse is on, within the current world and the screen.
	/// </summary>
	/// <returns>The cell position of the mouse, within the current active world and clamped to the screen boundaries.</returns>
	[PublicAPI]
	public static int ClampedMouseCell()
	{
		return Grid.PosToCell(ClampedMouseWorldPos());
	}

	/// <summary>
	/// Gets the world position of the mouse with a range randomly applied.
	/// </summary>
	/// <param name="range">The radius to randomly apply to the mouse position, in world space units (1 unit = 1 cell).</param>
	/// <returns>
	/// A random position within <paramref name="range"/> units of the world position of the mouse,
	/// within the current active world and clamped to the screen boundaries.
	/// </returns>
	[PublicAPI]
	public static Vector3 ClampedMousePosWithRange(int range)
	{
		if (Camera.main != null)
		{
			var clampedMouseScreenPos = ClampedMousePos();
			var worldPoint = Camera.main.ScreenToWorldPoint(ClampedMousePos());

			var theta = Random.value * Mathf.PI * 2;
			var radius = range * Mathf.Sqrt(Random.value);
			var randomOffset = radius * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
			var randomPoint = worldPoint + randomOffset;

			var currentWorldMin = ClusterManager.Instance.activeWorld.minimumBounds;
			var currentWorldMax = ClusterManager.Instance.activeWorld.maximumBounds;
			var clamped = new Vector3(
				Mathf.Clamp(randomPoint.x, currentWorldMin.x, currentWorldMax.x),
				Mathf.Clamp(randomPoint.y, currentWorldMin.y, currentWorldMax.y),
				randomPoint.z
			);
			return clamped;
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Gets the <see cref="Grid"/> cell of the mouse with a range randomly applied.
	/// </summary>
	/// <param name="range">The radius to randomly apply to the mouse position, in world space units (1 unit = 1 cell).</param>
	/// <returns>
	/// A random <see cref="Grid"/> cell within <paramref name="range"/> units of the world position of the mouse,
	/// within the current active world and clamped to the screen boundaries.
	/// </returns>
	[PublicAPI]
	public static int ClampedMouseCellWithRange(int range)
	{
		var pos = ClampedMousePosWithRange(range);
		return Grid.PosToCell(pos);
	}

	/// <summary>
	/// Gets a random <see cref="Grid"/> cell near the current mouse position.
	/// </summary>
	/// <returns>A random <see cref="Grid"/> cell in the current world that is near the mouse.</returns>
	[PublicAPI]
	public static int RandomCellNearMouse()
	{
		return ClampedMouseCellWithRange(5);
	}

	/// <summary>
	/// Gets the world position of the <see cref="Grid"/> cell that the mouse is on, within the current world and the screen.
	/// </summary>
	/// <returns>The world position of the cell that the mouse is in, within the current active world and clamped to the screen boundaries.</returns>
	public static Vector3 ClampedMouseCellWorldPos()
	{
		return Grid.CellToPos(ClampedMouseCell());
	}

	/// <summary>
	/// Gets the world position of the bottom left of the area shown by the camera.
	/// </summary>
	/// <returns>The world position of the bottom left of the area shown by the camera.</returns>
	[PublicAPI]
	public static Vector3 CameraMinWorldPos()
	{
		if (Camera.main is Camera main)
		{
			var ray = main.ViewportPointToRay(Vector3.zero);
			var currentWorldMin = ClusterManager.Instance.activeWorld.minimumBounds;
			var currentWorldMax = ClusterManager.Instance.activeWorld.maximumBounds;
			var point = ray.GetPoint(Mathf.Abs(ray.origin.z / ray.direction.z));
			return new Vector3(
				Mathf.Clamp(point.x, currentWorldMin.x, currentWorldMax.x),
				Mathf.Clamp(point.y, currentWorldMin.y, currentWorldMax.y),
				point.z
			);
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Gets the world position of the top right of the area shown by the camera.
	/// </summary>
	/// <returns>The world position of the top right of the area shown by the camera.</returns>
	[PublicAPI]
	public static Vector3 CameraMaxWorldPos()
	{
		if (Camera.main is Camera main)
		{
			var ray = main.ViewportPointToRay(Vector3.one);
			var currentWorldMin = ClusterManager.Instance.activeWorld.minimumBounds;
			var currentWorldMax = ClusterManager.Instance.activeWorld.maximumBounds;
			var point = ray.GetPoint(Mathf.Abs(ray.origin.z / ray.direction.z));
			return new Vector3(
				Mathf.Clamp(point.x, currentWorldMin.x, currentWorldMax.x),
				Mathf.Clamp(point.y, currentWorldMin.y, currentWorldMax.y),
				point.z
			);
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Interpolates between two vectors with a function that slows down as it approaches 1, but overshoots slightly and bounces back.
	/// </summary>
	/// <param name="a">The first vector.</param>
	/// <param name="b">The second vector.</param>
	/// <param name="x">The fraction of the transition to apply.</param>
	/// <returns>A vector that is interpolated following the bounce back function.</returns>
	[PublicAPI]
	public static Vector3 EaseOutLerp(Vector3 a, Vector3 b, float x)
	{
		return Vector3.LerpUnclamped(a, b, EaseOutBack(x));
	}

	/// <summary>
	/// A function that on the range [0,1], moves quickly towards 1, starts to slow down, but overshoots, and then comes back to 1.
	/// </summary>
	/// <param name="x">The input value to the function.</param>
	/// <returns>The output of the function.</returns>
	[PublicAPI]
	public static float EaseOutBack(float x)
	{
		const float c1 = 0.8f;
		const float c2 = c1 + 1;

		return 1 + c2 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
	}

	// Returns the mouse's position on the screen, not clamped to a world, but clamped to a screen
	private static Vector3 ClampedMousePos()
	{
		var pos = KInputManager.GetMousePos();
		return new Vector3(
			Mathf.Clamp(pos.x, 0, Screen.width),
			Mathf.Clamp(pos.y, 0, Screen.height),
			pos.z
		);
	}
}
