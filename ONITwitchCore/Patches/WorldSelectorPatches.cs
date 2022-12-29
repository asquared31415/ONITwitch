using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class WorldSelectorPatches
{
	[HarmonyPatch(typeof(WorldSelector), "SortRows")]
	public static class WorldSelector_SortRows_Patch
	{
		[UsedImplicitly]
		public static void Postfix(WorldSelector __instance)
		{
			SortRows(__instance.worldRows);
		}

		// returns the absolute depth of the current world, or the depth at which a loop happens, relative to this world 
		// 0 = top level
		private static int GetWorldDepth(IReadOnlyDictionary<int, WorldContainer> worldInfo, int worldIdx)
		{
			var depth = -1;
			var seenIndexes = new HashSet<int>();
			while ((worldIdx != ClusterManager.INVALID_WORLD_IDX) && !seenIndexes.Contains(worldIdx))
			{
				seenIndexes.Add(worldIdx);
				depth += 1;
				worldIdx = worldInfo[worldIdx].ParentWorldId;
			}

			return depth;
		}

		// collects all worlds, depth first, into an order list, then applies that order
		private static void SortRows(Dictionary<int, MultiToggle> rows)
		{
			// collect worlds because it's slow and we use it lots 
			var worlds = new Dictionary<int, WorldContainer>();
			foreach (var (worldIdx, _) in rows)
			{
				worlds.Add(worldIdx, ClusterManager.Instance.GetWorld(worldIdx));
			}

			// collect depths
			var worldIdxDepthMap = new Dictionary<int, int>();
			foreach (var (worldIdx, _) in rows)
			{
				var worldDepth = GetWorldDepth(worlds, worldIdx);
				worldIdxDepthMap.Add(worldIdx, worldDepth);
			}

			int CompareWorlds(int a, int b)
			{
				var worldA = worlds[a];
				var worldB = worlds[b];
				var aTime = worldA.IsModuleInterior ? float.PositiveInfinity : worldA.DiscoveryTimestamp;
				var bTime = worldB.IsModuleInterior ? float.PositiveInfinity : worldB.DiscoveryTimestamp;
				return aTime.CompareTo(bTime);
			}

			var worldIndexOrder = new List<int>();

			var rootWorlds = rows.Where(pair => worldIdxDepthMap[pair.Key] == 0).Select(pair => pair.Key).ToList();
			rootWorlds.Sort(CompareWorlds);
			// reverse because the stack will be processed in reverse order
			rootWorlds.Reverse();
			var worldStack = new Stack<int>(rootWorlds);
			while (worldStack.Count > 0)
			{
				var world = worldStack.Pop();
				worldIndexOrder.Add(world);

				var childWorlds = rows.Where(
						pair => !worldIndexOrder.Contains(pair.Key) &&
								(worlds[pair.Key].ParentWorldId == world)
					)
					.Select(pair => pair.Key)
					.ToList();
				childWorlds.Sort(CompareWorlds);

				// reverse because the pushes will be processed in reverse order
				childWorlds.Reverse();
				foreach (var childWorld in childWorlds)
				{
					worldStack.Push(childWorld);
				}
			}

			for (var idx = 0; idx < worldIndexOrder.Count; idx++)
			{
				var worldIdx = worldIndexOrder[idx];
				// order them properly
				rows[worldIdx].transform.SetSiblingIndex(idx);

				// set up indents
				var depth = worldIdxDepthMap[worldIdx];
				var hierarchy = rows[worldIdx].GetComponent<HierarchyReferences>();
				hierarchy.GetReference<RectTransform>("Indent").anchoredPosition = 32 * depth * Vector2.right;
				hierarchy.GetReference<RectTransform>("Status").anchoredPosition = -8 * depth * Vector2.right;
			}
		}
	}
}
