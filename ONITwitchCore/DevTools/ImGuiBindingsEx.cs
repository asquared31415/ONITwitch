using System;
using System.Runtime.InteropServices;
using UnityEngine;

// ReSharper disable UnusedMember.Global bindings

namespace ONITwitch.DevTools;

[Obsolete("Not to be used directly, add a nice wrapper to ImGuiInternal")]
internal static class ImGuiBindingsEx
{
	[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
	// Note: int is a bitflags, see imgui_internal.h for flag values
	public static extern void igPushItemFlag(int itemFlags, bool enabled);

	[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
	public static extern void igPopItemFlag();

	[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool igButtonEx(string label, Vector2 size, int flags = 0);
}
