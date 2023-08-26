using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps;
using ONITwitch.Patches;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;
using DataManager = ONITwitch.EventLib.DataManager;
using EventInfo = ONITwitch.EventLib.EventInfo;

namespace ONITwitch.DevTools.Panels;

internal class EventsPanel : IDevToolPanel
{
	// The delay in seconds before an event should be triggered.
	private float eventDelay;

	private List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)> eventEntries;

	// The filter that the user input to search for events.
	private string eventFilter = "";

	// TODO: fix this, the list probably isn't stable.
	// probably needs to be a string ID?
	private int selectedSurpriseBoxPrefabIdx;

	// Whether to override the random range around the mouse.
	// The value used is in `PosUtil.MouseRangeOverride`
	private bool useMouseRangeOverride;

	public void DrawPanel()
	{
		// Indent everything in the header
		ImGui.Indent();

		DrawEventDelay();

		ImGui.Checkbox("Override Mouse Range", ref useMouseRangeOverride);
		if (useMouseRangeOverride)
		{
			var range = PosUtil.MouseRangeOverride.HasValue ? PosUtil.MouseRangeOverride.Value : 5;
			ImGui.SliderInt("Range Override", ref range, 0, 20);
			PosUtil.MouseRangeOverride = range;
		}
		else
		{
			PosUtil.MouseRangeOverride = null;
		}

		ImGui.SliderFloat("Party Time Intensity", ref PartyTimePatch.Intensity, 0, 10);

		var validPrefabs = Assets.Prefabs.Where(SurpriseBox.PrefabIsValid).ToList();
		string previewName;
		if (selectedSurpriseBoxPrefabIdx >= validPrefabs.Count)
		{
			selectedSurpriseBoxPrefabIdx = 0;
			previewName = "";
		}
		else
		{
			previewName = Util.StripTextFormatting(validPrefabs[selectedSurpriseBoxPrefabIdx].name);
		}

		var spawnEnabled = selectedSurpriseBoxPrefabIdx < validPrefabs.Count;
		if (ImGuiInternal.ButtonEx(
				"Spawn Prefab",
				spawnEnabled
					? ImGuiInternal.ImGuiButtonFlags.None
					: ImGuiInternal.ImGuiButtonFlags.Internal_Disabled
			))
		{
			var prefab = validPrefabs[selectedSurpriseBoxPrefabIdx];
			GameScheduler.Instance.ScheduleNextFrame(
				"Spawn Surprise Box Prefab",
				_ => { SurpriseBox.SpawnPrefab(prefab, PosUtil.ClampedMouseCellWorldPos()); }
			);
		}

		ImGui.SameLine();
		if (ImGui.BeginCombo("##SurprisePrefabSelector", previewName))
		{
			for (var idx = 0; idx < validPrefabs.Count; idx++)
			{
				var isSelected = idx == selectedSurpriseBoxPrefabIdx;
				var name = Util.StripTextFormatting(validPrefabs[idx].name);
				if (ImGui.Selectable(name, isSelected))
				{
					selectedSurpriseBoxPrefabIdx = idx;
				}

				// make the selected item focus when the drop down is opened
				if (isSelected)
				{
					ImGui.SetItemDefaultFocus();
				}
			}

			ImGui.EndCombo();
		}

		ImGui.NewLine();

		ImGui.Text("Events with a");
		ImGui.SameLine();
		TwitchImGui.ColoredBullet(ColorUtil.GreenSuccessColor);
		ImGui.SameLine();
		ImGui.Text("icon have their condition currently met. Events with a");
		ImGui.SameLine();
		TwitchImGui.ColoredBullet(ColorUtil.RedWarningColor);
		ImGui.SameLine();
		ImGui.Text("do not.");

		// initialize the entries with no filter
		eventEntries ??= GenerateEventEntries(null);

		// When the filter changes, re-create the entries.
		if (ImGuiEx.InputFilter("Search##EventSearch", ref eventFilter, 100))
		{
			eventEntries = GenerateEventEntries(eventFilter);
		}

		var dataInst = DataManager.Instance;
		foreach (var (eventNamespace, groups) in eventEntries)
		{
			var mod = Global.Instance.modManager.mods.Find(mod => mod.staticID == eventNamespace);
			var headerName = Util.StripTextFormatting(mod != null ? mod.title : eventNamespace);

			var missingNamespace = headerName.IsNullOrWhiteSpace();
			if (missingNamespace)
			{
				ImGui.PushStyleColor(ImGuiCol.Text, Color.red);
				headerName = "MISSING NAMESPACE";
			}

			if (ImGui.CollapsingHeader(headerName))
			{
				ImGui.PushStyleColor(ImGuiCol.Text, Color.white);
				ImGui.Indent();

				var firstGroup = true;
				foreach (var (groupName, events) in groups)
				{
					if (firstGroup)
					{
						firstGroup = false;
					}
					else
					{
						ImGui.NewLine();
					}

					ImGui.Text(groupName);
					foreach (var eventInfo in events)
					{
						var condColor = eventInfo.CheckCondition(dataInst.GetDataForEvent(eventInfo))
							? ColorUtil.GreenSuccessColor
							: ColorUtil.RedWarningColor;
						TwitchImGui.ColoredBullet(condColor);
						var buttonPressed = ImGui.Button($"{eventInfo}##{eventInfo.Id}");

						ImGuiEx.TooltipForPrevious($"ID: {eventInfo.Id}");
						if (buttonPressed)
						{
							Log.Debug($"Dev Tool triggering Event {eventInfo} (id {eventInfo.Id})");
							var data = dataInst.GetDataForEvent(eventInfo);
							GameScheduler.Instance.Schedule(
								"dev trigger event",
								eventDelay,
								_ => { eventInfo.Trigger(data); }
							);
						}
					}
				}

				ImGui.Unindent();
				ImGui.PopStyleColor();
			}

			if (missingNamespace)
			{
				// pop the red style if we pushed it before
				ImGui.PopStyleColor();
			}
		}

		// unindent for end of events header 
		ImGui.Unindent();
	}

	private void DrawEventDelay()
	{
		ImGui.SliderFloat("Event Delay", ref eventDelay, 0, 60);
	}

	[MustUseReturnValue]
	[NotNull]
	private static List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)>
		GenerateEventEntries([CanBeNull] string filter)
	{
		bool MatchesFilter([NotNull] EventInfo info)
		{
			return string.IsNullOrWhiteSpace(filter) ||
				   (info.FriendlyName?.ToLowerInvariant().Contains(filter.ToLowerInvariant()) == true) ||
				   info.Id.ToLowerInvariant().Contains(filter.ToLowerInvariant());
		}

		var namespacedGroupedEvents = new Dictionary<string, Dictionary<string, List<EventInfo>>>();
		foreach (var eventGroup in TwitchDeckManager.Instance.GetGroups())
		{
			foreach (var (info, _) in eventGroup.GetWeights())
			{
				if (MatchesFilter(info))
				{
					var eventNamespace = info.EventNamespace;
					if (!namespacedGroupedEvents.ContainsKey(eventNamespace))
					{
						namespacedGroupedEvents[eventNamespace] = new Dictionary<string, List<EventInfo>>();
					}

					var groupName = info.Group.Name.Contains("__<nogroup>__") ? "__NoGroup" : info.Group.Name;
					if (!namespacedGroupedEvents[eventNamespace].ContainsKey(groupName))
					{
						namespacedGroupedEvents[eventNamespace][groupName] = new List<EventInfo>();
					}

					namespacedGroupedEvents[eventNamespace][groupName].Add(info);
				}
			}
		}

		// sort events by name and then ID
		int CompareInfo(EventInfo infoA, EventInfo infoB)
		{
			// if both have a friendly name, use that
			var nameA = infoA.FriendlyName;
			var nameB = infoB.FriendlyName;
			if ((nameA != null) && (nameB != null))
			{
				return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
			}

			// if neither have a friendly name, then compare IDs
			if ((nameA == null) && (nameB == null))
			{
				return string.Compare(infoA.Id, infoB.Id, StringComparison.OrdinalIgnoreCase);
			}

			// the event without a name is greater (last) 
			return nameA == null ? 1 : -1;
		}

		List<(string, List<EventInfo>)> SortGroupsForNamespace(Dictionary<string, List<EventInfo>> groups)
		{
			var sorted = groups.OrderBy(entry => entry.Key).Select(pair => (pair.Key, pair.Value)).ToList();
			foreach (var (_, events) in sorted)
			{
				events.Sort(CompareInfo);
			}

			return sorted;
		}

		var filtered = new List<(string Namespace, List<(string GroupName, List<EventInfo> Events)> GroupedEvents)>();

		// put the base mod events in first, then the rest, sorted by namespace
		if (namespacedGroupedEvents.TryGetValue(TwitchModInfo.StaticID, out var baseGroups))
		{
			namespacedGroupedEvents.Remove(TwitchModInfo.StaticID);
			filtered.Add((TwitchModInfo.StaticID, SortGroupsForNamespace(baseGroups)));
		}

		foreach (var (modNamespace, groups) in namespacedGroupedEvents)
		{
			filtered.Add((modNamespace, SortGroupsForNamespace(groups)));
		}

		return filtered;
	}
}
