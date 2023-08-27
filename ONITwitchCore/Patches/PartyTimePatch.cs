using System;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitch.Patches;

[HarmonyPatch(typeof(LightBufferCompositor), "OnRenderImage")]
internal static class PartyTimePatch
{
	// The default intensity for the party time effect.
	public const float DefaultIntensity = 1.6f;

	public static bool Enabled;

	// The intensity used for the party time effect, possibly overridden by the dev tools.
	public static float Intensity = DefaultIntensity;

	private static Material hsvMaterial;
	private static readonly int HueProperty = Shader.PropertyToID("_Hue");
	private static readonly int SaturationProperty = Shader.PropertyToID("_Saturation");

	[UsedImplicitly]
	private static void Postfix(RenderTexture src, RenderTexture dest)
	{
		if (hsvMaterial == null)
		{
			hsvMaterial = new Material(Shader.Find("Klei/PostFX/HueSaturation"));
		}

		if (Enabled)
		{
			const float cycleTime = 10;
			const double secToRadians = Math.PI * 2d / cycleTime;
			// Scale sin to [1/12,11/12]
			var h = 1f / 12f * (5f * Mathf.Sin((float) (secToRadians * Time.unscaledTime)) + 6);

			hsvMaterial.SetFloat(HueProperty, h);
			hsvMaterial.SetFloat(SaturationProperty, Intensity);

			Graphics.Blit(src, dest, hsvMaterial);
		}
	}
}
