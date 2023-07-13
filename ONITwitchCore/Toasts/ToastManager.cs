using System;
using JetBrains.Annotations;
using ONITwitch.Cmps;
using ONITwitch.Settings;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ONITwitch.Toasts;

/// <summary>
///     Provides methods for creating toasts.
/// </summary>
[PublicAPI]
public static class ToastManager
{
	private static GameObject canvas;

	/// <summary>
	///     Creates a toast with a tile and a body.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public static GameObject InstantiateToast([CanBeNull] string title, [CanBeNull] string body)
	{
		var go = InstantiateToastCommon(ModAssets.Toasts.NormalToastPrefab, title, body);
		if (go == null)
		{
			return null;
		}

		go.SetActive(true);
		return go;
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that targets a position when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="pos">The position to target on click.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos
	)
	{
		var go = InstantiateToastCommon(ModAssets.Toasts.ClickableToastPrefab, title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<OniTwitchToast>();

		toastCmp.Focus = OniTwitchToast.FocusKind.Position;
		toastCmp.FocusPos = pos;

		go.SetActive(true);
		return go;
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that targets a position when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="pos">The position to target on click.</param>
	/// <param name="orthographicSize">
	///     The orthographic size the camera should go to. Higher is more zoomed out. Must be
	///     strictly greater than 0.
	/// </param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithPosTargetAndZoom(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos,
		float orthographicSize
	)
	{
		if (orthographicSize <= 0)
		{
			throw new ArgumentException("Orthographic size must be positive", nameof(orthographicSize));
		}

		var go = InstantiateToastCommon(ModAssets.Toasts.ClickableToastPrefab, title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<OniTwitchToast>();

		toastCmp.Focus = OniTwitchToast.FocusKind.Position;
		toastCmp.FocusPos = pos;
		toastCmp.OrthographicSize = orthographicSize;

		go.SetActive(true);
		return go;
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that selects a <see cref="GameObject" /> when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="target">The <see cref="GameObject" /> to target on click.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	// METHOD NAME MUST NOT BE AMBIGUOUS
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target
	)
	{
		var go = InstantiateToastCommon(ModAssets.Toasts.ClickableToastPrefab, title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<OniTwitchToast>();

		toastCmp.Focus = OniTwitchToast.FocusKind.Object;
		toastCmp.FocusGo = target;

		go.SetActive(true);
		return go;
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that selects a <see cref="GameObject" /> when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="target">The <see cref="GameObject" /> to target on click.</param>
	/// <param name="orthographicSize">
	///     The orthographic size the camera should go to. Higher is more zoomed out. Must be
	///     strictly greater than 0.
	/// </param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithGoTargetAndZoom(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target,
		float orthographicSize
	)
	{
		if (orthographicSize <= 0)
		{
			throw new ArgumentException("Orthographic size must be positive", nameof(orthographicSize));
		}

		var go = InstantiateToastCommon(ModAssets.Toasts.ClickableToastPrefab, title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<OniTwitchToast>();

		toastCmp.Focus = OniTwitchToast.FocusKind.Object;
		toastCmp.FocusGo = target;
		toastCmp.OrthographicSize = orthographicSize;

		go.SetActive(true);
		return go;
	}

	[CanBeNull]
	private static GameObject InstantiateToastCommon(
		GameObject prefab,
		[CanBeNull] string title,
		[CanBeNull] string body
	)
	{
		if (!GenericModSettings.Data.ShowToasts)
		{
			return null;
		}

		if (canvas == null)
		{
			canvas = new GameObject("ToastCanvas");
			var canvasCmp = canvas.AddComponent<Canvas>();
			canvasCmp.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasCmp.sortingOrder = 100;
			canvasCmp.pixelPerfect = true;

			var scaler = canvas.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);

			canvas.AddComponent<GraphicRaycaster>();

			Object.DontDestroyOnLoad(canvas);
			canvas.SetActive(true);
		}

		var toast = Util.KInstantiateUI(prefab, canvas);
		var toastCmp = toast.AddOrGet<OniTwitchToast>();

		// Set keys to empty string to prevent localizing. The string passed in should be localized.
		toastCmp.Title = toast.transform.GetChild(0).GetChild(0).GetComponent<LocText>();
		toastCmp.Title.key = "";
		toastCmp.Title.text = title;
		toastCmp.Body = toast.transform.GetChild(1).GetChild(0).GetComponent<LocText>();
		toastCmp.Body.key = "";
		toastCmp.Body.text = body;

		toastCmp.Focus = OniTwitchToast.FocusKind.None;

		return toast;
	}
}
