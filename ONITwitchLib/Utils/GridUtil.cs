using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Logger;

namespace ONITwitchLib.Utils;

/// <summary>
/// Provides utilities for searching the <see cref="Grid"/>.
/// </summary>
[PublicAPI]
public static class GridUtil
{
	/// <summary>
	/// The directions that are neighbors to any cell.
	/// </summary>
	[PublicAPI] public static readonly List<Direction> CellNeighbors = new()
	{
		Direction.Down,
		Direction.Right,
		Direction.Left,
		Direction.Up,
	};

	/// <summary>
	/// Gets all neighbors of a cell that are valid and in the same world.
	/// </summary>
	/// <param name="cell">The base cell.</param>
	/// <returns>An enumerable of all cells that are in bounds neighbors of <paramref name="cell"/>.</returns>
	/// <seealso cref="CellNeighbors"/>
	/// <seealso cref="Grid.IsValidCell"/>
	/// <seealso cref="Grid.AreCellsInSameWorld"/>
	[PublicAPI]
	[NotNull]
	public static IEnumerable<int> GetNeighborsInBounds(int cell)
	{
		return CellNeighbors.Select(dir => Grid.GetCellInDirection(cell, dir))
			.Where(n => Grid.IsValidCell(n) && Grid.AreCellsInSameWorld(cell, n));
	}

	/// <summary>
	/// Gets the neighbors of a cell that do not have foundation.
	/// </summary>
	/// <param name="cell">The base cell.</param>
	/// <returns>An enumerable of cells that are neighbors of <paramref name="cell"/> and that are clear for foundation.</returns>
	/// <seealso cref="CellNeighbors"/>
	/// <seealso cref="IsCellFoundationEmpty"/>
	[PublicAPI]
	[NotNull]
	public static IEnumerable<int> GetNeighborsWithFoundationClearance(int cell)
	{
		return GetNeighborsInBounds(cell).Where(IsCellFoundationEmpty);
	}

	/// <summary>
	/// Gets whether a cell is empty of solids and foundation.
	/// </summary>
	/// <param name="cell">The cell to check.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="cell"/> is not a foundation, is a valid cell for building,
	/// and is not solid, otherwise <see langword="false"/>.
	/// </returns>
	/// <seealso cref="Grid.Foundation"/>
	/// <seealso cref="Grid.IsValidBuildingCell"/>
	/// <seealso cref="Grid.IsSolidCell"/>
	/// <seealso cref="IsCellFoundationEmpty"/>
	[PublicAPI]
	public static bool IsCellEmpty(int cell)
	{
		var isFoundation = Grid.Foundation[cell];
		var validCell = Grid.IsValidBuildingCell(cell);
		var isOpen = !Grid.IsSolidCell(cell);
		return !isFoundation && validCell && isOpen;
	}

	/// <summary>
	/// Gets whether a cell is empty for the purposes of foundation and if it is not maximum hardness.
	/// </summary>
	/// <param name="cell">The cell to check.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="cell"/> is not a foundation, is a valid cell for building,
	/// and is diggable, otherwise <see langword="false"/>.
	/// </returns>
	/// <seealso cref="Grid.Foundation"/>
	/// <seealso cref="Grid.IsValidBuildingCell"/>
	/// <seealso cref="Grid.Element"/>
	/// <seealso cref="IsCellEmpty"/>
	[PublicAPI]
	public static bool IsCellFoundationEmpty(int cell)
	{
		var isFoundation = Grid.Foundation[cell];
		var validCell = Grid.IsValidBuildingCell(cell);
		var isDiggable = Grid.Element[cell].hardness < byte.MaxValue;
		return !isFoundation && validCell && isDiggable;
	}

	/// <summary>
	/// Finds the nearest empty cell within <inheritdoc cref="NearestEmptyCellDepth"/> cells of <paramref name="baseCell"/>.
	/// </summary>
	/// <param name="baseCell">The cell to begin searching from.</param>
	/// <returns>
	/// The nearest cell that is not solid, within the same world as <paramref name="baseCell"/>,
	/// or <see cref="Grid.InvalidCell"/> (-1) if one cannot be found in range.
	/// </returns>
	/// <remarks>
	/// This search is based on <c>GameUtil.FloodFillFind</c>, which does a breadth first search,
	/// moving out approximately equally in all directions
	/// </remarks>
	/// <seealso cref="GameUtil.FloodFillFind{T}"/>
	/// <seealso cref="IsCellEmpty"/>
	[PublicAPI]
	public static int NearestEmptyCell(int baseCell)
	{
		var emptyCell = GameUtil.FloodFillFind<object>(
			(cell, _) => IsCellEmpty(cell) && Grid.AreCellsInSameWorld(cell, baseCell),
			null,
			baseCell,
			NearestEmptyCellDepth,
			false,
			false
		);

		if (!Grid.IsValidBuildingCell(emptyCell))
		{
			Log.Warn($"Unable to find empty cell close to {baseCell} output {emptyCell}");
		}

		return emptyCell;
	}

	/// <summary>
	/// Finds the nearest empty cell within <inheritdoc cref="NearestEmptyCellDepth"/> cells of <paramref name="baseCell"/>
	/// that also has all neighbors empty. 
	/// </summary>
	/// <param name="baseCell">The cell to begin searching from.</param>
	/// <returns>
	/// The nearest cell within the same world as <paramref name="baseCell"/> that is not solid and has all neighbors
	/// empty, or <see cref="Grid.InvalidCell"/> (-1) if one cannot be found in range.
	/// </returns>
	/// <remarks>
	/// This search is based on <c>GameUtil.FloodFillFind</c>, which does a breadth first search,
	/// moving out approximately equally in all directions
	/// </remarks>
	/// <seealso cref="GameUtil.FloodFillFind{T}"/>
	/// <seealso cref="IsCellEmpty"/>
	[PublicAPI]
	public static int FindCellWithCavityClearance(int baseCell)
	{
		var emptyCell = GameUtil.FloodFillFind<object>(
			(cell, _) =>
			{
				var cellEmpty = IsCellEmpty(cell);
				var neighborsValid = GetNeighborsInBounds(cell).All(IsCellEmpty);
				return cellEmpty && neighborsValid;
			},
			null,
			baseCell,
			NearestEmptyCellDepth,
			false,
			false
		);

		if (!Grid.IsValidCell(emptyCell))
		{
			Log.Warn($"Unable to find valid cell with clearance at start cell {baseCell}, output {emptyCell}");
		}

		return emptyCell;
	}

	/// <summary>
	/// Finds the nearest cell that does not have a foundation within <inheritdoc cref="NearestEmptyCellDepth"/> cells
	/// of <paramref name="baseCell"/> where its neighbors also all have no foundation. 
	/// </summary>
	/// <param name="baseCell">The cell to begin searching from.</param>
	/// <returns>
	/// The nearest cell within the same world as <paramref name="baseCell"/> that is not foundation and where all
	/// neighbors do not have foundation, or <see cref="Grid.InvalidCell"/> (-1) if one cannot be found in range.
	/// </returns>
	/// <remarks>
	/// This search is based on <c>GameUtil.FloodFillFind</c>, which does a breadth first search,
	/// moving out approximately equally in all directions
	/// </remarks>
	/// <seealso cref="GameUtil.FloodFillFind{T}"/>
	/// <seealso cref="IsCellFoundationEmpty"/>
	[PublicAPI]
	public static int FindCellWithFoundationClearance(int baseCell)
	{
		var emptyCell = GameUtil.FloodFillFind<object>(
			(cell, _) =>
			{
				var cellEmpty = IsCellFoundationEmpty(cell);
				var neighborsValid = GetNeighborsInBounds(cell).All(IsCellFoundationEmpty);
				return cellEmpty && neighborsValid;
			},
			null,
			baseCell,
			NearestEmptyCellDepth,
			false,
			false
		);

		if (!Grid.IsValidCell(emptyCell))
		{
			Log.Warn($"Unable to find valid cell with building clearance at start cell {baseCell}, output {emptyCell}");
		}

		return emptyCell;
	}

	/// <summary>
	/// Collects cells by flood fill but does not favor one direction first.
	/// Does not clear the set of found cells if too many cells match, it just stops early.
	/// </summary>
	/// <param name="startCell">The initial cell to start searching from.</param>
	/// <param name="cellValid">The function called to determine whether a visited cell is valid.</param>
	/// <param name="maxSize">The maximum number of cells to collect.</param>
	/// <param name="invalidVisitedCells">All of the cells that were visited that were not considered valid.</param>
	/// <returns>A HashSet containing the cells that were valid.</returns>
	[PublicAPI]
	[NotNull]
	public static HashSet<int> FloodCollectCells(
		int startCell,
		[NotNull] Func<int, bool> cellValid,
		int maxSize = 1000,
		[CanBeNull] HashSet<int> invalidVisitedCells = null
	)
	{
		var result = new HashSet<int>();

		const int cellsDefaultCapacity = 64;
		var visitedCells = new HashSet<int>();
		var toVisit = new Queue<int>(cellsDefaultCapacity);
		toVisit.Enqueue(startCell);

		// Once there are no more cells that can be visited, or the maximum number of cells has been reached, stop
		while ((toVisit.Count > 0) && (result.Count < maxSize))
		{
			var visitingCell = toVisit.Dequeue();
			visitedCells.Add(visitingCell);

			var isValidCell = cellValid(visitingCell);
			if (isValidCell)
			{
				result.Add(visitingCell);

				// Only ever queue the cells that have not yet been visited
				var neighbors = GetNeighborsInBounds(visitingCell).Where(cell => !visitedCells.Contains(cell));
				foreach (var unvisitedNeighbor in neighbors)
				{
					toVisit.Enqueue(unvisitedNeighbor);
				}
			}
			else
			{
				invalidVisitedCells?.Add(visitingCell);
			}
		}

		return result;
	}

	/// <summary>
	/// Iterates over a region of cells defined by a <see cref="Game.SimActiveRegion"/>.
	/// </summary>
	/// <param name="region">The region of cells to iterate.</param>
	/// <returns>An enumerable containing every cell within the region.</returns>
	[PublicAPI]
	[NotNull]
	public static IEnumerable<int> IterateCellRegion(Game.SimActiveRegion region)
	{
		return IterateCellRegion(region.region.first, region.region.second);
	}

	/// <summary>
	/// Iterates over a region of cells defined by a minimum and a maximum.
	/// </summary>
	/// <param name="min">The minimum bounds of the region.</param>
	/// <param name="max">The maximum bounds of the region.</param>
	/// <returns>An enumerable containing every cell within the region.</returns>
	[PublicAPI]
	[NotNull]
	public static IEnumerable<int> IterateCellRegion(Vector2I min, Vector2I max)
	{
		for (var y = min.y; y < max.y; y++)
		{
			for (var x = min.x; x < max.x; x++)
			{
				yield return Grid.XYToCell(x, y);
			}
		}
	}

	/// <summary>
	/// Iterates over all cells that the game considers currently active.
	/// </summary>
	/// <returns>An enumerable containing all active cells.</returns>
	/// <remarks>As of Mergedown, the game activates whole planets at a time, based on whether they are discovered.</remarks>
	[PublicAPI]
	[NotNull]
	public static IEnumerable<int> ActiveSimCells()
	{
		var activeSimRegions = SimRegionsDelegate(Game.Instance);
		return activeSimRegions.SelectMany(IterateCellRegion);
	}

	/// <summary>
	/// Finds the nearest cell that can have the passed <see cref="BuildingDef"/> built,
	/// or <see cref="Grid.InvalidCell"/> (-1) if a cell cannot be found.
	/// </summary>
	/// <param name="cell">The cell to begin searching.</param>
	/// <param name="building">The building to check placement for.</param>
	/// <param name="orientation">The orientation of the building.</param>
	/// <returns>
	/// The nearest cell to <paramref name="cell"/> that can support building <paramref name="building"/> with
	/// <paramref name="orientation"/> orientation.
	/// </returns>
	[PublicAPI]
	public static int FindCellOpenToBuilding(
		int cell,
		[NotNull] BuildingDef building,
		Orientation orientation = Orientation.Neutral
	)
	{
		var foundCell = GameUtil.FloodFillFind<object>(
			(testCell, _) => building.IsValidPlaceLocation(null, testCell, orientation, false, out var _) &&
							 building.IsValidBuildLocation(null, testCell, orientation, false, out var _),
			null,
			cell,
			1_000,
			false,
			false
		);

		return foundCell;
	}

	/// <summary>40</summary>
	private const int NearestEmptyCellDepth = 40;

	private static readonly AccessTools.FieldRef<Game, List<Game.SimActiveRegion>> SimRegionsDelegate =
		AccessTools.FieldRefAccess<Game, List<Game.SimActiveRegion>>(
			AccessTools.DeclaredField(typeof(Game), "simActiveRegions")
		);
}
