using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ONITwitch.Settings;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitch;

internal static class ModAssets
{
	private const string ToastManifestName = "ONITwitch.Resources.toast";
	private const string OptionsManifestName = "ONITwitch.Resources.twitch_options";

	public static void LoadAssets()
	{
		var fonts = new List<TMP_FontAsset>(Resources.FindObjectsOfTypeAll<TMP_FontAsset>());
		Fonts.NotoSans = fonts.FirstOrDefault(f => f.name == "NotoSans-Regular");
		Fonts.GrayStroke = fonts.FirstOrDefault(f => f.name == "GRAYSTROKE REGULAR SDF");

		var toastBundle = LoadBundle(ToastManifestName);
		if (toastBundle != null)
		{
			Toasts.NormalToastPrefab = toastBundle.LoadAsset<GameObject>("assets/singletoast.prefab");
			FixText(Toasts.NormalToastPrefab.transform.Find("TitleContainer").gameObject, Fonts.GrayStroke);
			FixText(Toasts.NormalToastPrefab.transform.Find("BodyContainer").gameObject, Fonts.NotoSans);
			Toasts.NormalToastPrefab.SetActive(false);

			Toasts.ClickableToastPrefab = toastBundle.LoadAsset<GameObject>("assets/clickabletoast.prefab");
			FixText(Toasts.ClickableToastPrefab.transform.Find("TitleContainer").gameObject, Fonts.GrayStroke);
			FixText(Toasts.ClickableToastPrefab.transform.Find("BodyContainer").gameObject, Fonts.NotoSans);
			Toasts.ClickableToastPrefab.SetActive(false);
		}

		var optionsBundle = LoadBundle(OptionsManifestName);
		if (optionsBundle != null)
		{
			Options.GenericOptionsPrefab = optionsBundle.LoadAsset<GameObject>("TwitchGenericOptionsUI.prefab");
			FixText(
				Options.GenericOptionsPrefab.transform.Find("TitleBar").gameObject,
				Fonts.GrayStroke,
				20
			);
			FixText(
				Options.GenericOptionsPrefab.transform.Find("Content").gameObject,
				Fonts.NotoSans
			);
			FixText(
				Options.GenericOptionsPrefab.transform.Find("Buttons").gameObject,
				Fonts.NotoSans,
				16
			);
			Options.GenericOptionsPrefab.SetActive(false);
			Options.GenericOptionsPrefab.AddOrGet<GenericModSettingsUI>();

			Options.ConfigPopup = optionsBundle.LoadAsset<GameObject>("TwitchConfigUI.prefab");
			FixText(Options.ConfigPopup.transform.Find("TitleBar").gameObject, Fonts.GrayStroke, 20);
			FixText(Options.ConfigPopup.transform.Find("Content").gameObject, Fonts.NotoSans);
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
			locText.key = content;
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

	public static class Fonts
	{
		public static TMP_FontAsset NotoSans;
		public static TMP_FontAsset GrayStroke;
	}
}
