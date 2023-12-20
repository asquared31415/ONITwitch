using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using JetBrains.Annotations;
using ONITwitch.EventLib;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitch.DevTools.Panels;

internal class WeightsPanel : IDevToolPanel
{
	private const int NameColumnId = 1;
	private const int WeightColumnId = 2;

	private int totalWeight;
	[NotNull] private List<EventWithWeight> weightsList = [];

	public void DrawPanel()
	{
		// Generate weights, then only update when sorting changes.
		if (weightsList.Count == 0)
		{
			GenerateWeights();
		}

		ImGui.Text($"Total Weight: {totalWeight}");
		const ImGuiTableFlags flags = ImGuiTableFlags.Hideable | ImGuiTableFlags.Resizable |
									  ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable |
									  ImGuiTableFlags.SortMulti | ImGuiTableFlags.SortTristate | ImGuiTableFlags.RowBg |
									  ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY;
		if (ImGui.BeginTable("twitch_event_weights", 2, flags, new Vector2(0f, 20 * 12)))
		{
			// The first column should be fixed width and the second column should stretch to the right side of the table.
			// The table should start with the weight column default sorted descending.
			ImGui.TableSetupColumn(
				"Name",
				ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortAscending,
				0f,
				NameColumnId
			);
			ImGui.TableSetupColumn(
				"Weight",
				ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.DefaultSort |
				ImGuiTableColumnFlags.PreferSortDescending,
				0f,
				WeightColumnId
			);
			// Keeps labels always visible.
			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			// Update the sorting when the sort specs change.
			unsafe
			{
				var sortSpecsPtr = ImGui.TableGetSortSpecs();
				if ((IntPtr) sortSpecsPtr.NativePtr != IntPtr.Zero)
				{
					if (sortSpecsPtr.SpecsDirty)
					{
						weightsList.Sort((a, b) => SortItemsWithSpecs(sortSpecsPtr.Specs, a, b));
						sortSpecsPtr.SpecsDirty = false;
					}
				}
			}

			// Use clipper to only process the items that are actually being shown.
			unsafe
			{
				var clipper = new ImGuiListClipper();
				var clipperPtr = new ImGuiListClipperPtr(&clipper);
				clipperPtr.Begin(weightsList.Count);
				while (clipperPtr.Step())
				{
					for (var idx = clipperPtr.DisplayStart; idx < clipperPtr.DisplayEnd; idx++)
					{
						var eventWithWeight = weightsList[idx];
						ImGui.TableNextRow();

						ImGui.TableNextColumn();
						ImGui.Text($"{eventWithWeight.EventInfo}");

						ImGui.TableNextColumn();
						var fraction = (float) eventWithWeight.Weight / totalWeight;
						ImGui.Text($"{eventWithWeight.Weight} ({fraction * 100:F2}%)");
					}
				}
			}

			ImGui.EndTable();
		}
	}

	private void GenerateWeights()
	{
		var weightMap = new Dictionary<EventInfo, int>();

		foreach (var eventGroup in TwitchDeckManager.Instance.GetGroups())
		{
			foreach (var (eventInfo, weight) in eventGroup.GetWeights())
			{
				totalWeight += weight;
				if (weightMap.ContainsKey(eventInfo))
				{
					Log.Warn($"Event {eventInfo} appeared in groups more than once");
					weightMap[eventInfo] += weight;
				}
				else
				{
					weightMap.Add(eventInfo, weight);
				}
			}
		}

		weightsList = weightMap.Select(
				static pair => new EventWithWeight
				{
					EventInfo = pair.Key,
					Weight = pair.Value,
				}
			)
			.ToList();
	}

	private static int SortItemsWithSpecs(
		RangeAccessor<ImGuiTableColumnSortSpecs> specs,
		EventWithWeight a,
		EventWithWeight b
	)
	{
		// Iterate over all sort specs because they can contain multiple things to sort by.
		// The first spec that has a difference between two items will return the ordering.
		// This means it's "sort by first spec, then by second, then by...".
		// Not a List or enumerable, so it must be manually indexed.
		for (var idx = 0; idx < specs.Count; idx += 1)
		{
			var spec = specs[idx];
			var ordering = spec.ColumnUserID switch
			{
				NameColumnId => string.Compare(a.EventInfo.Id, b.EventInfo.Id, StringComparison.Ordinal),
				WeightColumnId => a.Weight.CompareTo(b.Weight),
				// ReSharper disable once NotResolvedInText (not actually an arg).
				_ => throw new ArgumentOutOfRangeException("ColumnUserID"),
			};

			if (ordering != 0)
			{
				// Swap the ordering if it's sort descending.
				if (spec.SortDirection == ImGuiSortDirection.Descending)
				{
					return -ordering;
				}

				return ordering;
			}
		}

		// The items were equal for all specs.
		return 0;
	}

	private struct EventWithWeight
	{
		internal EventInfo EventInfo;
		internal int Weight;
	}
}
