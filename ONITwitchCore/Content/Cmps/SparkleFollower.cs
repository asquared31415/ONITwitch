using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ONITwitch.Content.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class SparkleFollower : KMonoBehaviour, ISim200ms
{
	[Serialize] [SerializeField] public int numSparkles = 5;
	[Serialize] [SerializeField] public Vector2 offset = Vector2.zero;

	private readonly List<GameObject> sparkles = new();

	public void Sim200ms(float dt)
	{
		foreach (var sparkle in sparkles)
		{
			var rand = (Vector3) Random.insideUnitCircle / 8f;
			sparkle.transform.position = transform.position + (Vector3) offset + rand;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();

		for (var i = 0; i < numSparkles; i++)
		{
			sparkles.Add(
				GameUtil.KInstantiate(
					EffectPrefabs.Instance.SparkleStreakFX,
					transform.position,
					Grid.SceneLayer.FXFront
				)
			);
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
