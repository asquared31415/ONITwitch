using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitch.Patches;

internal static class TranslationPatches
{
	[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
	// ReSharper disable once InconsistentNaming
	private static class Localization_Initialize_Patch
	{
		// https://forums.kleientertainment.com/forums/topic/123339-guide-for-creating-translatable-mods

		[UsedImplicitly]
		private static void Postfix()
		{
			var root = typeof(STRINGS);

			// register strings with namespace
			Localization.RegisterForTranslation(root);

			// Register strings without namespace
			LocString.CreateLocStringKeys(root, null);

			// Load user created translation files
			LoadOverrideStrings();

			// Creates template for users to edit
			Localization.GenerateStringsTemplate(root, Path.Combine(KMod.Manager.GetDirectory(), "strings_templates"));
		}

		private static void LoadOverrideStrings()
		{
			var locale = Localization.GetLocale();
			if (locale != null)
			{
				var path = Path.Combine(TwitchModInfo.MainModFolder, "translations", locale.Code + ".po");
				if (File.Exists(path))
				{
					Log.Debug($"Loading translation for locale {locale.Code}");
					var stringsFile = Localization.LoadStringsFile(path, false);
					Localization.OverloadStrings(stringsFile);
				}
			}
		}
	}
}
