using System;
using HarmonyLib;
using KSerialization;
using ONITwitchCore.Cmps;
using UnityEngine;

namespace ONITwitchCore.Integration.DecorPackA;

[SerializationConfig(MemberSerialization.OptIn)]
// used to access data from GlitterLight2D off the mood lamps
public class GlitterMoodLampAccessor : KMonoBehaviour
{
	private static readonly Type GlitterLampType =
		Type.GetType("DecorPackA.Buildings.MoodLamp.GlitterLight2D, DecorPackA");

	// Duration in seconds for the mood lamp to cycle
	private static readonly float Duration =
		(float) AccessTools.DeclaredField(GlitterLampType, "DURATION").GetValue(null);

	// Elapsed time in seconds
	private static readonly AccessTools.FieldRef<object, float> ElapsedAccess =
		AccessTools.FieldRefAccess<float>(GlitterLampType, "elapsed");

	private KMonoBehaviour glitterLight2DComponent;
	private Operational operational;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Debug.Log(Duration);
		glitterLight2DComponent = (KMonoBehaviour) gameObject.GetComponent(GlitterLampType);
		operational = gameObject.GetComponent<Operational>();

		// Add a tracker to the lamps too
		var tracker = new GameObject("Glitter Puft Tracker");
		tracker.AddComponent<GlitterPuftTracker>();
		tracker.transform.SetParent(transform, false);
	}

	public bool IsActiveGlitterLamp()
	{
		return glitterLight2DComponent.enabled && operational.IsOperational;
	}

	public float GetFractionElapsed()
	{
		if (!IsActiveGlitterLamp())
		{
			return 0.0f;
		}

		return ElapsedAccess(glitterLight2DComponent) / Duration;
	}
}
