using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitch.Patches;

/// <summary>
///     The cluster location sensor tries to add every world that is not an interior to the selection screen.
///     Pocket dimensions are not a rocket interior (and cannot be), but they should not be added to the location sensor
///     since that causes crashes and does not make sense anyway.
/// </summary>
internal static class ClusterLocationFilterSideScreenPatches
{
	[HarmonyPatch(typeof(ClusterLocationFilterSideScreen), "Build")]
	// ReSharper disable once InconsistentNaming
	private static class ClusterLocationFilterSideScreen_Build_Patch
	{
		private static readonly MethodInfo IsModuleInteriorGetter = AccessTools.PropertyGetter(
			typeof(WorldContainer),
			nameof(WorldContainer.IsModuleInterior)
		);

		[UsedImplicitly]
		[NotNull]
		private static IEnumerable<CodeInstruction> Transpiler(
			[NotNull] [ItemNotNull] IEnumerable<CodeInstruction> orig,
			ILGenerator generator
		)
		{
			var codes = orig.ToList();

			var isInteriorIdx = codes.FindIndex(ci => ci.Calls(IsModuleInteriorGetter));
			if (isInteriorIdx == -1)
			{
				Log.Warn("Unable to find `IsModuleInterior` in `ClusterLocationFilterSideScreen.Build`");
				return codes;
			}

			// Insert a dup before the IsModuleInterior, to get an extra copy of the WorldContainer on the stack.
			codes.Insert(isInteriorIdx++, new CodeInstruction(OpCodes.Dup));

			// Then after the IsModuleInterior, insert a call to a helper method that checks whether the world is a
			// pocket dimension.
			// Skip over the IsModuleInterior to insert after.
			isInteriorIdx += 1;

			// Stack is
			// bool IsModuleInterior
			// WorldContainer
			// But we need access to the world, so swap them.

			// Get a local to use for the swap.
			var isInteriorLocal = generator.DeclareLocal(typeof(bool));
			codes.Insert(isInteriorIdx++, new CodeInstruction(OpCodes.Stloc, isInteriorLocal));

			codes.Insert(
				isInteriorIdx++,
				CodeInstruction.CallClosure<Func<WorldContainer, bool>>(
					static ([CanBeNull] world) =>
					(world != null) && world.gameObject.HasTag(ExtraTags.PocketDimensionEntityTag)
				)
			);

			// Put the `bool` from IsModuleInterior back on the stack.
			codes.Insert(isInteriorIdx++, new CodeInstruction(OpCodes.Ldloc, isInteriorLocal));

			// The following jump skips over the problematic code if `IsModuleInterior` is true, so logically or the
			// pocket dimension check with that to also skip.
			codes.Insert(isInteriorIdx, new CodeInstruction(OpCodes.Or));

			return codes;
		}
	}
}
