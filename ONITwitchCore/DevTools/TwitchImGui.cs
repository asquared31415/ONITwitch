using ImGuiNET;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitch.DevTools;

internal static class TwitchImGui
{
	public static void WithStyle([CanBeNull] ImGuiStyle style, [NotNull] System.Action action)
	{
		if (style == null)
		{
			action();
		}
		else
		{
			style.PushStyle();
			action();
			style.PopStyle();
		}
	}

	public static void ColoredBullet(Color color)
	{
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.Bullet();
		ImGui.PopStyleColor();
	}
}
