using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps;
using UnityEngine;

namespace ONITwitch.Patches;

internal static class EclipsePatches
{
	[HarmonyPatch(typeof(GameClock), nameof(GameClock.IsNighttime))]
	// ReSharper disable once InconsistentNaming
	private static class GameClock_IsNighttime_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(ref bool __result)
		{
			if ((Game.Instance != null) && Game.Instance.TryGetComponent(out OniTwitchEclipse eclipse))
			{
				if (eclipse.State == OniTwitchEclipse.EclipseState.Eclipse)
				{
					__result = true;
				}
			}
		}
	}

	[HarmonyPatch(typeof(TimeOfDay), "UpdateVisuals")]
	// ReSharper disable once InconsistentNaming
	private static class TimeOfDay_UpdateVisuals_Patch
	{
		private static readonly int TimeOfDayProperty = Shader.PropertyToID("_TimeOfDay");

		private static readonly Func<TimeOfDay, float> UpdateSunlightIntensity =
			(Func<TimeOfDay, float>) Delegate.CreateDelegate(
				typeof(Func<TimeOfDay, float>),
				null,
				AccessTools.Method(typeof(TimeOfDay), "UpdateSunlightIntensity")
			);

		[UsedImplicitly]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private static bool Prefix(TimeOfDay __instance, ref float ___scale)
		{
			// if in an eclipse, redo the math to lerp to 0 scale and force the sunlight intensity to 0
			if (Game.Instance.TryGetComponent(out OniTwitchEclipse eclipse) &&
				(eclipse.State == OniTwitchEclipse.EclipseState.Eclipse))
			{
				// scale of 1 is max normal effect
				___scale = Mathf.Lerp(___scale, 1.5f, Time.deltaTime * 0.05f);
				var sunlight = UpdateSunlightIntensity(__instance);
				Shader.SetGlobalVector(TimeOfDayProperty, new Vector4(___scale, sunlight, 0.0f, 0.0f));

				return false;
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(BeeHive), nameof(BeeHive.InitializeStates))]
	// ReSharper disable once InconsistentNaming
	private static class BeeHive_InitializeStates_Patch
	{
		[UsedImplicitly]
		// ReSharper disable once InconsistentNaming
		private static void Postfix(BeeHive __instance)
		{
			__instance.enabled.grownStates.nightTime.transitions.Clear();
			__instance.enabled.grownStates.nightTime.EventTransition(
				GameHashes.NewDay,
				_ => GameClock.Instance,
				__instance.enabled.grownStates.dayTime,
				_ =>
				{
					// don't let the transition happen if there is an eclipse
					if (Game.Instance.TryGetComponent(out OniTwitchEclipse eclipse) &&
						(eclipse.State == OniTwitchEclipse.EclipseState.Eclipse))
					{
						return false;
					}

					// original condition, presumably so that multi-day time transitions work
					return GameClock.Instance.GetTimeSinceStartOfCycle() <= 1.0;
				}
			);
		}
	}
}
