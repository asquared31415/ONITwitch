using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

namespace ONITwitch.DevTools;

internal class ImGuiStyle
{
	private readonly Dictionary<ImGuiCol, Color> colors = new();
	private readonly Dictionary<ImGuiStyleVar, float> floatStyles = new();
	private readonly Dictionary<ImGuiStyleVar, Vector2> vectorStyles = new();

	public ImGuiStyle AddColor(ImGuiCol col, Color color)
	{
		colors[col] = color;
		return this;
	}

	public ImGuiStyle AddStyle(ImGuiStyleVar styleVar, float val)
	{
		floatStyles[styleVar] = val;
		return this;
	}

	public ImGuiStyle AddStyle(ImGuiStyleVar styleVar, Vector2 val)
	{
		vectorStyles[styleVar] = val;
		return this;
	}

	public ImGuiStyle RemoveColor(ImGuiCol col)
	{
		colors.Remove(col);
		return this;
	}

	public ImGuiStyle RemoveStyle(ImGuiStyleVar styleVar)
	{
		floatStyles.Remove(styleVar);
		vectorStyles.Remove(styleVar);

		return this;
	}

	public void PushStyle()
	{
		foreach (var (col, color) in colors)
		{
			ImGui.PushStyleColor(col, color);
		}

		foreach (var (styleVar, val) in floatStyles)
		{
			ImGui.PushStyleVar(styleVar, val);
		}

		foreach (var (styleVar, val) in vectorStyles)
		{
			ImGui.PushStyleVar(styleVar, val);
		}
	}

	public void PopStyle()
	{
		ImGui.PopStyleColor(colors.Count);
		ImGui.PopStyleVar(floatStyles.Count + vectorStyles.Count);
	}
}
