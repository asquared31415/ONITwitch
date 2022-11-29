using JetBrains.Annotations;
using ONITwitchCore.Cmps;
using ONITwitchCore.Config;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore.Toasts;

public static class ToastManager
{
	private static GameObject canvas;

	[CanBeNull]
	private static GameObject InstantiateToastCommon([CanBeNull] string title, [CanBeNull] string body)
	{
		if (MainConfig.Instance.ConfigData.DisableToasts)
		{
			return null;
		}

		if (canvas == null)
		{
			canvas = new GameObject("ToastCanvas");
			var canvasCmp = canvas.AddComponent<Canvas>();
			canvasCmp.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasCmp.sortingOrder = 99999;
			canvasCmp.pixelPerfect = true;

			var scaler = canvas.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);

			canvas.AddComponent<GraphicRaycaster>();

			Object.DontDestroyOnLoad(canvas);
			canvas.SetActive(true);
		}

		var toast = Util.KInstantiateUI(ModAssets.Toasts.ToastPrefab, canvas);
		var toastCmp = toast.AddOrGet<Toast>();
		toastCmp.Title = toast.transform.GetChild(0).GetChild(0).GetComponent<Text>();
		toastCmp.Title.text = title;
		toastCmp.Body = toast.transform.GetChild(1).GetChild(0).GetComponent<Text>();
		toastCmp.Body.text = body;
		toastCmp.Focus = Toast.FocusKind.None;

		return toast;
	}

	/// <summary>
	/// Creates a toast with a tile and a body.
	/// </summary>
	/// <param name="title">The title for the toast</param>
	/// <param name="body">The body for the toast</param>
	/// <returns>The newly created toast's <see cref="GameObject"/>.</returns>
	[CanBeNull]
	public static GameObject InstantiateToast([CanBeNull] string title, [CanBeNull] string body)
	{
		var go = InstantiateToastCommon(title, body);
		if (go == null)
		{
			return null;
		}

		go.SetActive(true);
		return go;
	}

	/// <summary>
	/// Creates a toast with a tile and a body, that targets a position when clicked.
	/// </summary>
	/// <param name="title">The title for the toast</param>
	/// <param name="body">The body for the toast</param>
	/// <param name="pos">The position to target on click</param>
	/// <returns>The newly created toast's <see cref="GameObject"/>.</returns>
	[CanBeNull]
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos
	)
	{
		var go = InstantiateToastCommon(title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<Toast>();

		toastCmp.Focus = Toast.FocusKind.Position;
		toastCmp.FocusPos = pos;

		go.SetActive(true);
		return go;
	}

	/// <summary>
	/// Creates a toast with a tile and a body, that selects a <see cref="GameObject"/> when clicked.
	/// </summary>
	/// <param name="title">The title for the toast</param>
	/// <param name="body">The body for the toast</param>
	/// <param name="target">The <see cref="GameObject"/> to target on click</param>
	/// <returns>The newly created toast's <see cref="GameObject"/>.</returns>
	[CanBeNull]
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target
	)
	{
		var go = InstantiateToastCommon(title, body);
		if (go == null)
		{
			return null;
		}

		var toastCmp = go.GetComponent<Toast>();

		toastCmp.Focus = Toast.FocusKind.Object;
		toastCmp.FocusGo = target;

		go.SetActive(true);
		return go;
	}
}
