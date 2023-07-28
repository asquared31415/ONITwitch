using System;
using HarmonyLib;
using KSerialization;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitch.Settings.Components;

[SerializationConfig(MemberSerialization.OptIn)]
internal class ModsButtonNotification : KMonoBehaviour
{
	/// <summary>
	///     The message to display on the button.
	/// </summary>
	[NonSerialized] public string Message;

	protected override void OnSpawn()
	{
		base.OnSpawn();

		var mainMenu = GetComponent<MainMenu>();
		var buttonParent = Traverse.Create(mainMenu).Field<GameObject>("buttonParent").Value;

		// find the mods button and update its text
		var childCount = buttonParent.transform.childCount;
		for (var idx = 0; idx < childCount; idx++)
		{
			var childButton = buttonParent.transform.GetChild(idx);
			if (childButton != null)
			{
				// this is sub-optimal, but there's not any other identifier for the buttons
				var modLocText = childButton.GetComponentInChildren<LocText>();
				if ((modLocText != null) && modLocText.text.StartsWith(UI.FRONTEND.MODS.TITLE))
				{
					const float messageHeight = 30;

					// add a buffer space for the newly added message
					if (childButton.TryGetComponent(out LayoutElement childLayout))
					{
						childLayout.minHeight += messageHeight;
						childLayout.preferredHeight += messageHeight;
					}
					else
					{
						Log.Warn("Unable to expand mods button, could not find its LayoutElement.");
					}

					// align the original text to the very top, with its original preferred height
					// this helps to preserve compatibility with Mod Updater and similar
					var childTextRect = modLocText.GetComponent<RectTransform>();
					childTextRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 64);

					var textGo = new GameObject("TwitchUpdateText");
					textGo.transform.SetParent(childButton, false);
					// don't be active so that the LocText doesn't crash trying to use a null key
					textGo.SetActive(false);
					var text = textGo.AddComponent<LocText>();
					// set the key to empty string since we're using a value here
					text.key = "";
					text.text = Message;
					text.font = ModAssets.Fonts.GrayStroke;
					text.fontStyle = FontStyles.Normal;
					text.fontSize = 14;

					textGo.AddOrGet<TmpPostInit>().alignment = TextAlignmentOptions.Center;

					var rect = textGo.GetComponent<RectTransform>();
					// Place the object at the bottom of the button, no padding, with the height that we reserved
					rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, messageHeight);
					// expand anchors to fill the width
					rect.anchorMin = rect.anchorMin with { x = 0 };
					rect.anchorMax = rect.anchorMax with { x = 1 };
					// and make the rect not expand past that width
					rect.sizeDelta = rect.sizeDelta with { x = 0 };

					textGo.SetActive(true);
					Log.Debug("Added text to mods button");
					break;
				}
			}
		}
	}
}
