using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Settings;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class ModsScreenPatches
{
	[HarmonyPatch(typeof(ModsScreen), "BuildDisplay")]
	// ReSharper disable once InconsistentNaming
	private static class ModsScreen_BuildDisplay_Patch
	{
		[UsedImplicitly]
		// object because its a List<private struct>
		// ReSharper disable once InconsistentNaming
		private static void Postfix(object ___displayedMods)
		{
			AddSettingsButton((IEnumerable) ___displayedMods);
		}

		private static readonly FieldInfo ModIdxField = AccessTools.Field(
			Type.GetType("ModsScreen+DisplayedMod, Assembly-CSharp"),
			"mod_index"
		);

		private static readonly FieldInfo RectTransformField = AccessTools.Field(
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
					text.text = STRINGS.ONITWITCH.UI.SETTINGS.BUTTON_NAME;
					text.fontSize = 14;
				}
			}
		}

		private static void OpenModSettingsScreen()
		{
			Util.KInstantiateUI(
					ModAssets.Options.GenericOptionsPrefab,
					MainMenu.Instance.transform.parent.gameObject
				)
				.SetActive(true);
		}
	}
}
