using System.IO;
using System.Reflection;
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

			// register manually first so that the override works
			AccessTools.Method(typeof(Localization), "AddAssembly", new[] { typeof(string), typeof(Assembly) })
				.Invoke(null, new object[] { "ONITwitch", typeof(OniTwitchMod).Assembly });

			// Load user created translation files
			// load before creating keys so that `Strings` has the right values
			LoadOverrideStrings();

			// register strings with namespace
			LocString.CreateLocStringKeys(root, "ONITwitch.");
			// Register strings without namespace
			LocString.CreateLocStringKeys(root, null);

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
