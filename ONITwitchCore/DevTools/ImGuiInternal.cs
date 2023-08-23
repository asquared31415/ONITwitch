using System;
using UnityEngine;

// Lots of things are copied even if unused, for completeness.
// ReSharper disable UnusedMember.Global
// Names copied from imgui.
// ReSharper disable InconsistentNaming

// We use "obsolete" to warn on usage of the raw bindings anywhere besides here
#pragma warning disable CS0618

namespace ONITwitch.DevTools;

internal static class ImGuiInternal
{
	[Flags]
	public enum ImGuiButtonFlags
	{
		None = 0,
		MouseButtonLeft = 1 << 0, // [Default] React on left mouse button
		MouseButtonRight = 1 << 1, // React on right mouse button
		MouseButtonMiddle = 1 << 2, // React on middle mouse button

		// return true on click (mouse down event)
		Internal_PressedOnClick = 1 << 4,

		// [Default] return true on click + release on same item <-- this is what the majority of Button are using
		Internal_PressedOnClickRelease = 1 << 5,

		// return true on click + release even if the release event is not done while hovering the item
		Internal_PressedOnClickReleaseAnywhere = 1 << 6,

		// return true on release (default requires click+release)
		Internal_PressedOnRelease = 1 << 7,

		// return true on double-click (default requires click+release)
		Internal_PressedOnDoubleClick = 1 << 8,

		// return true when held into while we are drag and dropping another item (used by e.g. tree nodes, collapsing headers)
		Internal_PressedOnDragDropHold = 1 << 9,
		Internal_Repeat = 1 << 10, // hold to repeat
		Internal_FlattenChildren = 1 << 11, // allow interactions even if a child window is overlapping

		// require previous frame HoveredId to either match id or be null before being usable, use along with SetItemAllowOverlap()
		Internal_AllowItemOverlap = 1 << 12,
		Internal_DontClosePopups = 1 << 13, // disable automatically closing parent popup on press // [UNUSED]
		Internal_Disabled = 1 << 14, // disable interactions

		// vertically align button to match text baseline - ButtonEx() only // FIXME: Should be removed and handled by SmallButton(), not possible currently because of DC.CursorPosPrevLine
		Internal_AlignTextBaseLine = 1 << 15,
		Internal_NoKeyModifiers = 1 << 16, // disable mouse interaction if a key modifier is held

		// don't set ActiveId while holding the mouse (ImGuiButtonFlags_PressedOnClick only)
		Internal_NoHoldingActiveId = 1 << 17,

		// don't override navigation focus when activated (FIXME: this is essentially used everytime an item uses
		// ImGuiItemFlags_NoNav, but because legacy specs don't requires LastItemData to be set ButtonBehavior(),
		// we can't poll g.LastItemData.InFlags)
		Internal_NoNavFocus = 1 << 18,
		Internal_NoHoveredOnFocus = 1 << 19, // don't report as hovered when nav focus is on this item

		// don't set key/input owner on the initial click (note: mouse buttons are keys! often, the key in question will be ImGuiKey_MouseLeft!)
		Internal_NoSetKeyOwner = 1 << 20,

		// don't test key/input owner when polling the key (note: mouse buttons are keys! often, the key in question will be ImGuiKey_MouseLeft!)
		Internal_NoTestKeyOwner = 1 << 21,
	}

	[Flags]
	public enum ImGuiItemFlags
	{
		None = 0,
		NoTabStop = 1 << 0, // Disable keyboard tabbing. This is a "lighter" version of ImGuiItemFlags_NoNav.

		// Button() will return true multiple times based on io.KeyRepeatDelay and io.KeyRepeatRate settings.
		ButtonRepeat = 1 << 1,

		// Disable interactions but doesn't affect visuals. See BeginDisabled()/EndDisabled(). See github.com/ocornut/imgui/issues/211
		Disabled = 1 << 2,

		// Disable any form of focusing (keyboard/gamepad directional navigation and SetKeyboardFocusHere() calls)
		NoNav = 1 << 3,
		NoNavDefaultFocus = 1 << 4, // Disable item being a candidate for default focus (e.g. used by title bar items)
		SelectableDontClosePopup = 1 << 5, // Disable MenuItem/Selectable() automatically closing their popup window

		// [BETA] Represent a mixed/indeterminate value, generally multi-selection where values differ. Currently only supported by Checkbox() (later should support all sorts of widgets)
		Internal_MixedValue = 1 << 6,
		Internal_ReadOnly = 1 << 7, // [ALPHA] Allow hovering interactions but underlying value is not changed.
		Internal_NoWindowHoverableCheck = 1 << 8, // Disable hoverable check in ItemHoverable()
	}

	public static void PushItemFlags(ImGuiItemFlags flags, bool enabled)
	{
		ImGuiBindingsEx.igPushItemFlag((int) flags, enabled);
	}

	public static bool ButtonEx(string label, ImGuiButtonFlags flags)
	{
		return ImGuiBindingsEx.igButtonEx(label, Vector2.zero, (int) flags);
	}
}
