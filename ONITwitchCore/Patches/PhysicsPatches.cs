using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class PhysicsPatches
{
	[HarmonyPatch(typeof(MainMenu), "OnSpawn")]
	// ReSharper disable once InconsistentNaming
	private static class MainMenu_OnSpawn_Patch
	{
		[UsedImplicitly]
		private static void Postfix()
		{
			// Ignore all collisions with the glitter puft layer except for itself, where we need to re-enable it
			for (var i = 0; i < 32; i++)
			{
				if (i != GlitterPuftTracker.GlitterPuftLayerNumber)
				{
					Physics2D.IgnoreLayerCollision(GlitterPuftTracker.GlitterPuftLayerNumber, i);
				}
				else
				{
					Physics2D.IgnoreLayerCollision(GlitterPuftTracker.GlitterPuftLayerNumber, i, false);
				}
			}
		}
	}
}
