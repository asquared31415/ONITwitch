using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ONITwitchCore.Toasts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore;

public static class ModAssets
{
	private const string ToastManifestName = "ONITwitchCore.Resources.toast";

	public static class Toasts
	{
		public static GameObject ToastPrefab;
	}

	public static TMP_FontAsset NotoSans;
	public static TMP_FontAsset GrayStroke;

	public static void LoadAssets()
	{
		var fonts = new List<TMP_FontAsset>(Resources.FindObjectsOfTypeAll<TMP_FontAsset>());
		NotoSans = fonts.FirstOrDefault(f => f.name == "NotoSans-Regular");
		GrayStroke = fonts.FirstOrDefault(f => f.name == "GRAYSTROKE REGULAR SDF");

		var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ToastManifestName);
		if (assetStream == null)
		{
			Debug.LogWarning($"[Twitch Integration] Unable to locate embedded asset {ToastManifestName}");
		}

		var bundle = AssetBundle.LoadFromStream(assetStream);
		if (bundle == null)
		{
			Debug.LogWarning($"[Twitch Integration] Error loading asset bundle {ToastManifestName}");
		}

		Toasts.ToastPrefab = bundle.LoadAsset<GameObject>("assets/singletoast.prefab");
		FixText(Toasts.ToastPrefab.transform.Find("TitleContainer").gameObject, GrayStroke);
		FixText(Toasts.ToastPrefab.transform.Find("BodyContainer").gameObject, NotoSans);
		Toasts.ToastPrefab.SetActive(false);
	}

	private static void FixText(GameObject root, TMP_FontAsset font)
	{
		var texts = root.GetComponentsInChildren<Text>();

		foreach (var text in texts)
		{
			var go = text.gameObject;
			Object.DestroyImmediate(text);
			var locText = go.AddComponent<LocText>();
			locText.font = font;
			locText.fontStyle = FontStyles.Normal;
			locText.fontSize = 14;
			locText.enableWordWrapping = true;

			var postInit = go.AddOrGet<TmpPostInit>();
			postInit.alignment = TextAlignmentOptions.Top;
		}
	}
}
