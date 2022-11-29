using System.Collections.Generic;
using System.Linq;
using KSerialization;
using ONITwitchLib;
using TUNING;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONITwitchCore.Cmps;

// Most of the gradient and color logic is taken from Aki's Glitter Puft mood light
[SerializationConfig(MemberSerialization.OptIn)]
public class GlitterPuft : KMonoBehaviour, ISim33ms
{
	// group of the *trackers* not of the pufts
	public readonly List<GameObject> PuftGroup = new();
	public const float BaseFrequency = 1f / 10f;

	[Serialize] [SerializeField] private float t;

#pragma warning disable 649
	[MyCmpGet] private KBatchedAnimController animController;
	[MyCmpGet] private Light2D light;
#pragma warning restore 649

	private const int NumSparkles = 2;
	private readonly List<GameObject> sparkles = new();

	// The frequency to target, is modified when in a group to target the group
	private float targetFrequency = BaseFrequency;

	private Gradient gradient;
	private GradientColorKey[] colorKey;
	private GradientAlphaKey[] alphaKey;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		gradient = new Gradient();

		var colorList = new List<Color>
		{
			new(1.25f, 0.36f, 0.36f),
			new(1.25f, 0.92f, 0.36f),
			new(0.62f, 1.15f, 0.64f),
			new(0.44f, 1.25f, 1f),
			new(0.44f, 0.79f, 1.25f),
			new(1.10f, 0.74f, 1.12f),
		};

		colorKey = new GradientColorKey[colorList.Count];
		for (var index = 0; index < colorList.Count; ++index)
		{
			colorKey[index] = new GradientColorKey(colorList[index], (index + 1f) / colorList.Count);
		}

		alphaKey = new GradientAlphaKey[1];
		alphaKey[0].alpha = 1f;
		alphaKey[0].time = 0.0f;

		gradient.SetKeys(colorKey, alphaKey);

		for (var i = 0; i < NumSparkles; i++)
		{
			sparkles.Add(
				GameUtil.KInstantiate(
					EffectPrefabs.Instance.SparkleStreakFX,
					transform.position,
					Grid.SceneLayer.Front
				)
			);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();

		// Give a random offset to any pufts that have no timer set
		// workaround for old pufts and initial spawning
		if (t == 0)
		{
			t = Random.value;
		}

		SetColorFromTime(t);
	}

	public void Sim33ms(float dt)
	{
		// If there's a group nearby, try to synchronize with them
		if (PuftGroup.Count > 0)
		{
			// Get the sum of differences in phases of all visible pufts in range
			// By using the sum rather than the average, this lets larger groups have more effect
			// But because the sum is used, the multiplier has to be slightly lower to prevent overshooting
			var sumPhaseDiffs = 0f;
			var selfCell = Grid.PosToCell(gameObject);
			foreach (var otherPuft in PuftGroup.Select(otherTracker => otherTracker.transform.parent))
			{
				if ((otherPuft != null) && otherPuft.TryGetComponent<GlitterPuft>(out var otherGlitter))
				{
					if (Grid.VisibilityTest(selfCell, Grid.PosToCell(otherPuft.gameObject)))
					{
						sumPhaseDiffs += MathUtils.ShortestDistanceModuloOne(t, otherGlitter.t);
					}
				}
			}

			Debug.Log($"Sum phase diffs {sumPhaseDiffs}");

			const float phaseDiffAdjustmentMul = 0.05f;

			// Targeting a frequency based off the distance to other pufts lets them all come together
			// The multiplier is tuning, higher values cause larger changes, but may cause instability or overshooting
			targetFrequency = BaseFrequency + phaseDiffAdjustmentMul * sumPhaseDiffs;
		}
		else
		{
			targetFrequency = BaseFrequency;
		}

		Debug.Log($"target freq: {targetFrequency}");

		t += dt * targetFrequency;
		t %= 1;

		SetColorFromTime(t);
	}

	private void SetColorFromTime(float time)
	{
		// Go from 0 to 0.5, then back down to 0 linearly
		var adjusted = time > 0.5 ? 1 - time : time;

		var color = gradient.Evaluate(adjusted * 2f);

		animController.TintColour = color;
		// prevent the light beams from being way too bright
		light.Color = 0.5f * color;
		light.overlayColour = color * LIGHT2D.LIGHT_OVERLAY;

		foreach (var sparkle in sparkles)
		{
			var rand = (Vector3) Random.insideUnitCircle / 8f;
			sparkle.transform.position = transform.position + rand;
		}
	}

	protected override void OnCleanUp()
	{
		foreach (var sparkle in sparkles)
		{
			Destroy(sparkle);
		}

		sparkles.Clear();

		base.OnCleanUp();
	}
}
