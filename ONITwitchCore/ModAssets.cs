using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ONITwitchCore.Toasts;
using ONITwitchLib;
using ONITwitchLib.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ONITwitchCore;

internal static class ModAssets
{
	private const string ToastManifestName = "ONITwitchCore.Resources.toast";
	private const string OptionsManifestName = "ONITwitchCore.Resources.twitch_options";

	public static class Toasts
	{
		public static GameObject NormalToastPrefab;
		public static GameObject ClickableToastPrefab;
	}

	public static class Options
	{
		public static GameObject GenericOptionsPrefab;
		public static GameObject ConfigPopup;
	}

	private static TMP_FontAsset notoSans;
	private static TMP_FontAsset grayStroke;

	public static void LoadAssets()
	{
		var fonts = new List<TMP_FontAsset>(Resources.FindObjectsOfTypeAll<TMP_FontAsset>());
		notoSans = fonts.FirstOrDefault(f => f.name == "NotoSans-Regular");
		grayStroke = fonts.FirstOrDefault(f => f.name == "GRAYSTROKE REGULAR SDF");

		var toastBundle = LoadBundle(ToastManifestName);
		if (toastBundle != null)
		{
			Toasts.NormalToastPrefab = toastBundle.LoadAsset<GameObject>("assets/singletoast.prefab");
			FixText(Toasts.NormalToastPrefab.transform.Find("TitleContainer").gameObject, grayStroke);
			FixText(Toasts.NormalToastPrefab.transform.Find("BodyContainer").gameObject, notoSans);
			Toasts.NormalToastPrefab.SetActive(false);
			
			Toasts.ClickableToastPrefab = toastBundle.LoadAsset<GameObject>("assets/clickabletoast.prefab");
			FixText(Toasts.ClickableToastPrefab.transform.Find("TitleContainer").gameObject, grayStroke);
			FixText(Toasts.ClickableToastPrefab.transform.Find("BodyContainer").gameObject, notoSans);
			Toasts.ClickableToastPrefab.SetActive(false);
		}

		var optionsBundle = LoadBundle(OptionsManifestName);
		if (optionsBundle != null)
		{
			Options.GenericOptionsPrefab = optionsBundle.LoadAsset<GameObject>("TwitchGenericOptionsUI.prefab");
			FixText(
				Options.GenericOptionsPrefab.transform.Find("TitleBar").gameObject,
				grayStroke,
				20
			);
			FixText(
				Options.GenericOptionsPrefab.transform.Find("Content").gameObject,
				notoSans
			);
			FixText(
				Options.GenericOptionsPrefab.transform.Find("Buttons").gameObject,
				notoSans,
				16
			);
			Options.GenericOptionsPrefab.SetActive(false);

			Options.ConfigPopup = optionsBundle.LoadAsset<GameObject>("TwitchConfigUI.prefab");
			FixText(Options.ConfigPopup.transform.Find("TitleBar").gameObject, grayStroke, 20);
			FixText(Options.ConfigPopup.transform.Find("Content").gameObject, notoSans);
			Options.ConfigPopup.SetActive(false);
		}
	}

	private static AssetBundle LoadBundle(string name)
	{
		var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		if (assetStream == null)
		{
			Log.Warn($"Unable to locate embedded asset {name}");
			return null;
		}

		var bundle = AssetBundle.LoadFromStream(assetStream);
		if (bundle == null)
		{
			Log.Warn($"Error loading asset bundle {name}");
			return null;
		}

		return bundle;
	}

	private static void FixText(
		GameObject root,
		TMP_FontAsset font,
		float fontSize = 14
	)
	{
		var texts = root.GetComponentsInChildren<Text>();

		foreach (var text in texts)
		{
			var content = text.text;
			var color = text.color;
			var alignment = Traverse.Create(text).Property<TextAnchor>("alignment").Value;
			var go = text.gameObject;
			Object.DestroyImmediate(text);
			var locText = go.AddComponent<LocText>();
			locText.text = content;
			locText.font = font;
			locText.fontStyle = FontStyles.Normal;
			locText.fontSize = fontSize;
			locText.enableWordWrapping = true;
			locText.color = color;

			var postInit = go.AddOrGet<TmpPostInit>();
			postInit.alignment = GetAlignment(alignment);
		}
	}

	private static TextAlignmentOptions GetAlignment(TextAnchor anchor)
	{
		return anchor switch
		{
			TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
			TextAnchor.UpperCenter => TextAlignmentOptions.Top,
			TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
			TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
			TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
			TextAnchor.MiddleRight => TextAlignmentOptions.Right,
			TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
			TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
			TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
			_ => TextAlignmentOptions.Left,
		};
	}
}
