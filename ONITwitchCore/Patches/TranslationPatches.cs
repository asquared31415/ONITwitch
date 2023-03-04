using System.IO;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;

namespace ONITwitchCore.Patches;

public static class TranslationPatches
{
	[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
	public static class Localization_Initialize_Patch
	{
		// https://forums.kleientertainment.com/forums/topic/123339-guide-for-creating-translatable-mods

		[UsedImplicitly]
		public static void Postfix()
		{
			var root = typeof(STRINGS);

			// we need to manually register under the `ONITwitch` namespace because that's what lots of IDs assume
			// but it's not the assembly's namespace, so this has to be manually added so that the assembly gets updated
			AccessTools.Method(
					typeof(Localization),
					"AddAssembly",
					new[]
					{
						typeof(string),
						typeof(Assembly),
					}
				)
				.Invoke(
					null,
					new object[]
					{
						"ONITwitch",
						typeof(STRINGS).Assembly,
					}
				);
			LocString.CreateLocStringKeys(root, "ONITwitch.");

			// Load user created translation files
			LoadOverrideStrings();

			// Register strings without namespace
			// because we already loaded user translations, custom languages will overwrite these
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
					Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
				}
			}
		}
	}
}
