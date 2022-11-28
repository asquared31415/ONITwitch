using UnityEngine;

namespace ONITwitchLib;

public static class PosUtil
{
	// Returns the mouse's position on the screen, not clamped to a world
	private static Vector3 ClampedMousePos()
	{
		var pos = KInputManager.GetMousePos();
		return new Vector3(
			Mathf.Clamp(pos.x, 0, Screen.width),
			Mathf.Clamp(pos.y, 0, Screen.height),
			pos.z
		);
	}

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

	public static int ClampedMouseCell()
	{
		return Grid.PosToCell(ClampedMouseWorldPos());
	}

	public static Vector3 ClampedMousePosWithRange(int range)
	{
		if (Camera.main != null)
		{
			var clampedMouseScreenPos = ClampedMousePos();
			var worldPoint = Camera.main.ScreenToWorldPoint(ClampedMousePos());
			var randomOffset = range * Random.insideUnitCircle;
			var randomPoint = worldPoint + (Vector3) randomOffset;

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

	public static int ClampedMouseCellWithRange(int range)
	{
		var pos = ClampedMousePosWithRange(range);
		return Grid.PosToCell(pos);
	}

	public static int RandomCellNearMouse()
	{
		return ClampedMouseCellWithRange(5);
	}

	public static Vector3 ClampedMouseCellWorldPos()
	{
		return Grid.CellToPos(ClampedMouseCell());
	}

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

	public static Vector3 EaseOutLerp(Vector3 a, Vector3 b, float x)
	{
		return Vector3.LerpUnclamped(a, b, EaseOutBack(x));
	}

	public static Vector2 EaseOutLerp(Vector2 a, Vector2 b, float x)
	{
		return Vector2.LerpUnclamped(a, b, EaseOutBack(x));
	}

	public static float EaseOutLerp(float a, float b, float x)
	{
		return Mathf.LerpUnclamped(a, b, EaseOutBack(x));
	}

	public static float EaseOutBack(float x)
	{
		const float c1 = 0.8f;
		const float c2 = c1 + 1;

		return 1 + c2 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
	}
}
