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

	[CanBeNull]
	public static GameObject InstantiateToast([CanBeNull] string title, [CanBeNull] string body)
	{
		var toast = InstantiateToastCommon(title, body);
		toast.SetActive(true);
		return toast;
	}

	[CanBeNull]
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 target
	)
	{
		var go = InstantiateToastCommon(title, body);
		var toastCmp = go.GetComponent<Toast>();

		toastCmp.Focus = Toast.FocusKind.Position;
		toastCmp.FocusPos = target;

		go.SetActive(true);
		return go;
	}

	[CanBeNull]
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target
	)
	{
		var go = InstantiateToastCommon(title, body);
		var toastCmp = go.GetComponent<Toast>();

		toastCmp.Focus = Toast.FocusKind.Object;
		toastCmp.FocusGo = target;

		go.SetActive(true);
		return go;
	}
}
