using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Settings;
using ONITwitchLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ONITwitchCore.Patches;

public static class ModsScreenPatches
{
	private static GameObject canvas;

	[HarmonyPatch(typeof(ModsScreen), "BuildDisplay")]
	public static class ModsScreen_BuildDisplay_Patch
	{
		[UsedImplicitly]
		// object because its a List<private struct>
		public static void Postfix(object ___displayedMods)
		{
			AddSettingsButton((IEnumerable) ___displayedMods);
		}

		private static FieldInfo ModIdxField = AccessTools.Field(
			Type.GetType("ModsScreen+DisplayedMod, Assembly-CSharp"),
			"mod_index"
		);

		private static FieldInfo RectTransformField = AccessTools.Field(
			Type.GetType("ModsScreen+DisplayedMod, Assembly-CSharp"),
			"rect_transform"
		);

		private static void AddSettingsButton(IEnumerable displayedMods)
		{
			var mods = Global.Instance.modManager.mods;
			foreach (var modEntry in displayedMods)
			{
				var idx = (int) ModIdxField.GetValue(modEntry);
				var mod = mods[idx];
				// should only get here if it is active, but that might have issues with deactivating
				if ((mod.staticID == TwitchModInfo.StaticID) && mod.IsEnabledForActiveDlc())
				{
					var rectTransform = (RectTransform) RectTransformField.GetValue(modEntry);
					var buttonPrefab = rectTransform.Find("ManageButton").gameObject;

					var buttonGo = Util.KInstantiateUI(buttonPrefab, rectTransform.gameObject, true);
					var button = buttonGo.AddOrGet<KButton>();
					button.onClick += OpenModSettingsScreen;
					button.transform.SetSiblingIndex(4);
					var text = button.GetComponentInChildren<LocText>();
					text.text = STRINGS.UI.SETTINGS.BUTTON_NAME;
					text.fontSize = 14;
				}
			}
		}

		private static void OpenModSettingsScreen()
		{
			if (canvas == null)
			{
				canvas = new GameObject("SettingsCanvas");
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

			var settings = Util.KInstantiateUI(ModAssets.Options.GenericOptionsPrefab, canvas);
			settings.AddOrGet<GenericModSettingsUI>();
			settings.SetActive(true);
		}
	}
}
