using System.Runtime.InteropServices;

namespace ONITwitch;

internal partial class TwitchDevTool
{
	private static class ImGuiBindingsEx
	{
		[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
		// Note: int is a bitflags, see imgui_internal.h for flag values
		public static extern void igPushItemFlag(int itemFlags, bool enabled);

		[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
		public static extern void igPopItemFlag();
	}
}
