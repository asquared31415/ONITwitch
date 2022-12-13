using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Content.Entities;

namespace ONITwitchCore.Patches;

public static class SandboxSpawnerPatches
{
	[HarmonyPatch(typeof(SandboxToolParameterMenu), "ConfigureEntitySelector")]
	public static class SandboxToolParameterMenu_ConfigureEntitySelector_Patch
	{
		[UsedImplicitly]
		public static void Postfix(SandboxToolParameterMenu __instance)
		{
			var twitchSearchFilter = new SandboxToolParameterMenu.SelectorValue.SearchFilter(
				"Twitch Integration",
				entity =>
				{
					if (entity is KPrefabID kPrefabID)
					{
						if (kPrefabID.PrefabID() == PocketDimensionGeneratorConfig.Id)
						{
							return true;
						}
					}

					return false;
				}
			);

			var addedPrefabs = Assets.Prefabs.Where(prefab => twitchSearchFilter.condition(prefab)).ToList();

			var optionsList = __instance.entitySelector.options.ToList();
			optionsList.AddRange(addedPrefabs);
			__instance.entitySelector.options = optionsList.ToArray();

			var searchFiltersList = __instance.entitySelector.filters.ToList();
			searchFiltersList.Add(twitchSearchFilter);
			__instance.entitySelector.filters = searchFiltersList.ToArray();
		}
	}
}
