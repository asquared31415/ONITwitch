using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchLib.Utils;

public static class WorldUtil
{
	public static WorldContainer CreateWorldWithTemplate(
		[NotNull] GameObject worldGo,
		Vector2I size,
		[NotNull] string template,
		[CanBeNull] Action<WorldContainer> callback = null
	)
	{
		if (Grid.GetFreeGridSpace(size, out var offset))
		{
			var clusterInst = Traverse.Create(ClusterManager.Instance);
			var worldId = clusterInst.Method("GetNextWorldId").GetValue<int>();
			worldGo.AddOrGet<WorldInventory>();
			var worldContainer = worldGo.AddOrGet<WorldContainer>();
			worldContainer.SetRocketInteriorWorldDetails(worldId, size, offset);
			Traverse.Create(worldContainer).Field<bool>("isModuleInterior").Value = false;

			var worldEnd = offset + size;
			for (var y = offset.y; y < worldEnd.y; ++y)
			{
				for (var x = offset.x; x < worldEnd.x; ++x)
				{
					var cell = Grid.XYToCell(x, y);
					Grid.WorldIdx[cell] = (byte) worldId;
					Pathfinding.Instance.AddDirtyNavGridCell(cell);
				}
			}

			worldContainer.PlaceInteriorTemplate(template, () => { callback?.Invoke(worldContainer); });
			ClusterManager.Instance.Trigger((int) GameHashes.WorldAdded, worldContainer.id);

			return worldContainer;
		}

		Debug.LogError($"[TwitchIntegration] Was not able to create a world of size {size}");
		return null;
	}

	private delegate void RefreshTogglesDelegate(WorldSelector instance);

	[CanBeNull] private static RefreshTogglesDelegate refreshToggles;

	// Refreshes the specified world index in the world selector, or refreshes the entire selector if no index is provided
	public static void RefreshWorldSelector(int? worldIndex)
	{
		refreshToggles ??=
			AccessTools.MethodDelegate<RefreshTogglesDelegate>(
				AccessTools.Method(typeof(WorldSelector), "RefreshToggles")
			);

		if (!worldIndex.HasValue)
		{
			// The delegate has always been instantiated at this point, the AccessTools method will not return null
			refreshToggles!.Invoke(WorldSelector.Instance);
		}
		else
		{
			var idx = worldIndex.Value;
			var world = ClusterManager.Instance.GetWorld(idx);
			if (world == null)
			{
				Debug.LogWarning($"[Twitch Lib] Attempted to delete null world at index {idx}");
				return;
			}

			if (!world.TryGetComponent<ClusterGridEntity>(out var gridEntity))
			{
				Debug.LogWarning(
					$"[Twitch Lib] World {world} (idx {idx}) was not a cluster grid entity, this should never happen!"
				);
				return;
			}

			if (!WorldSelector.Instance.worldRows.TryGetValue(idx, out var worldRow))
			{
				Debug.LogWarning($"[Twitch Lib] World selector did not have a row for world {world} (idx {idx})");
				return;
			}

			var targetSprite = gridEntity.GetUISprite();
			if (targetSprite == null)
			{
				Debug.LogWarning($"[Twitch Lib] World {world} (idx {idx}) returned null UI sprite");
				return;
			}

			var hierarchy = worldRow.GetComponent<HierarchyReferences>();
			hierarchy.GetReference<Image>("Icon").sprite = targetSprite;
			hierarchy.GetReference<LocText>("Label").SetText(gridEntity.Name);
		}
	}

	[CanBeNull] private static AccessTools.FieldRef<ColonyDiagnosticUtility, Dictionary<int, List<ColonyDiagnostic>>>
		diagnosticDict;

	public static void AddDiagnostic(
		int worldIdx,
		ColonyDiagnostic diagnostic,
		ColonyDiagnosticUtility.DisplaySetting displaySetting = ColonyDiagnosticUtility.DisplaySetting.AlertOnly
	)
	{
		diagnosticDict ??=
			AccessTools.FieldRefAccess<ColonyDiagnosticUtility, Dictionary<int, List<ColonyDiagnostic>>>(
				AccessTools.Field(typeof(ColonyDiagnosticUtility), "worldDiagnostics")
			);

		var dict = diagnosticDict(ColonyDiagnosticUtility.Instance);
		if (!dict.TryGetValue(worldIdx, out var diagnostics) || (diagnostics == null))
		{
			Debug.LogWarning($"[Twitch Integration] World {worldIdx} did not exist in the diagnostics list");
			return;
		}

		diagnostics.Add(diagnostic);

		// Raw [] getter on this dictionary is fine, it should always be added by ColonyDiagnosticUtility.AddWorld and that would be checked above
		if (!ColonyDiagnosticUtility.Instance.diagnosticCriteriaDisabled[worldIdx].ContainsKey(diagnostic.id))
		{
			ColonyDiagnosticUtility.Instance.diagnosticCriteriaDisabled[worldIdx]
				.Add(diagnostic.id, new List<string>());
		}

		// Raw [] getter on this dictionary is fine, it should always be added by ColonyDiagnosticUtility.AddWorld and that would be checked above
		if (!ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[worldIdx].ContainsKey(diagnostic.id))
		{
			ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[worldIdx].Add(diagnostic.id, displaySetting);
		}
	}
}
