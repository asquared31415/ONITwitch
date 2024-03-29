using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchLib.Utils;

/// <summary>
///     Utilities for working with worlds.
/// </summary>
[PublicAPI]
public static class WorldUtil
{
	[CanBeNull] private static RefreshTogglesDelegate refreshToggles;

	[CanBeNull] private static AccessTools.FieldRef<ColonyDiagnosticUtility, Dictionary<int, List<ColonyDiagnostic>>>
		diagnosticDict;

	/// <summary>
	///     Allocates <see cref="Grid" /> space for a world and creates the world with a template, calling the callback once
	///     the template is placed.
	/// </summary>
	/// <param name="worldGo">The object that is an instance of a <see cref="ClusterGridEntity" /> to set up as the world.</param>
	/// <param name="size">The size of the world to create, in cells.</param>
	/// <param name="template">The template to place in the world.</param>
	/// <param name="callback">If present, the callback to call after placing the template.</param>
	/// <returns>The <see cref="WorldContainer" /> for the newly created world.</returns>
	/// <remarks>
	///     Important notes for modder usage:<br />
	///     The <c>WorldContainer</c> does not have an <c>overworldCell</c> set. You should set that field
	///     and add the cell to <c>SaveLoader.Instance.clusterDetailSave.overworldCells</c>.<br />
	///     None of the <c>Grid</c> is revealed. Use <c>GridVisibility.Reveal</c> or <c>Grid.Reveal</c> to reveal the grid.
	/// </remarks>
	[PublicAPI]
	[CanBeNull]
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

			// ReSharper disable PossibleLossOfFraction
			var pos = new Vector2(size.x / 2 + offset.x, size.y / 2 + offset.y);
			// ReSharper restore PossibleLossOfFraction
			TemplateLoader.Stamp(
				TemplateCache.GetTemplate(template),
				pos,
				() => { callback?.Invoke(worldContainer); }
			);

			ClusterManager.Instance.Trigger((int) GameHashes.WorldAdded, worldContainer.id);

			return worldContainer;
		}

		Log.Warn($"Unable to create a world of size {size}");
		return null;
	}

	/// <summary>
	///     Frees grid space for a world. This is a version of Grid.FreeGridSpace that actually functions to clear some
	///     properties of the cells, to avoid bugs where the solid property is not properly cleared.
	/// </summary>
	/// <param name="size">The size of the space to clear, in cells.</param>
	/// <param name="offset">The starting offset of the space to clear, in cells.</param>
	public static void FreeGridSpace(Vector2I size, Vector2I offset)
	{
		for (var y = offset.y; y < size.y + offset.y + 1; ++y)
		{
			for (var x = offset.x - 1; x < size.x + offset.x + 1; ++x)
			{
				var cell = Grid.XYToCell(x, y);
				if (Grid.IsValidCell(cell))
				{
					SimMessages.ReplaceElement(cell, SimHashes.Vacuum, null, 0);
					Grid.PreventFogOfWarReveal[cell] = false;
					SimMessages.ModifyCellWorldZone(cell, byte.MaxValue);
				}
			}
		}

		SimMessages.SimDataFreeCells(size.x, size.y, offset.x, offset.y);
		Game.Instance.roomProber.Refresh();
	}

	/// <summary>
	///     Refreshes the specified world index in the world selector, or refreshes the entire selector if no index is
	///     provided.
	/// </summary>
	/// <param name="worldIndex">The index of the world to refresh.</param>
	[PublicAPI]
	public static void RefreshWorldSelector([CanBeNull] int? worldIndex = null)
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
				Log.Debug($"Attempted to delete null world at index {idx}");
				return;
			}

			if (!world.TryGetComponent<ClusterGridEntity>(out var gridEntity))
			{
				Log.Debug($"World {world} (idx {idx}) was not a cluster grid entity, this should never happen!");
				return;
			}

			if (!WorldSelector.Instance.worldRows.TryGetValue(idx, out var worldRow))
			{
				Log.Debug($"World selector did not have a row for world {world} (idx {idx})");
				return;
			}

			var targetSprite = gridEntity.GetUISprite();
			if (targetSprite == null)
			{
				Log.Debug($"World {world} (idx {idx}) returned null UI sprite");
				return;
			}

			var hierarchy = worldRow.GetComponent<HierarchyReferences>();
			hierarchy.GetReference<Image>("Icon").sprite = targetSprite;
			hierarchy.GetReference<LocText>("Label").SetText(gridEntity.Name);
		}
	}

	/// <summary>
	///     Adds a diagnostic to a world.
	/// </summary>
	/// <param name="worldIdx">The index of the world to add to.</param>
	/// <param name="diagnostic">The diagnostic to add.</param>
	/// <param name="displaySetting">When to display the diagnostic in the world selector.</param>
	[PublicAPI]
	public static void AddDiagnostic(
		int worldIdx,
		[NotNull] ColonyDiagnostic diagnostic,
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
			Log.Debug($"World {worldIdx} did not exist in the diagnostics list");
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

	private delegate void RefreshTogglesDelegate(WorldSelector instance);
}
