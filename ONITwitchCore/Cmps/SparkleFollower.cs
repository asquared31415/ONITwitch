using System.Collections.Generic;
using KSerialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class SparkleFollower : KMonoBehaviour, ISim200ms
{
	[Serialize] [SerializeField] public int NumSparkles = 5;
	[Serialize] [SerializeField] public Vector2 Offset = Vector2.zero;

	private readonly List<GameObject> sparkles = new();

	protected override void OnSpawn()
	{
		base.OnSpawn();

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

	public void Sim200ms(float dt)
	{
		foreach (var sparkle in sparkles)
		{
			var rand = (Vector3) Random.insideUnitCircle / 8f;
			sparkle.transform.position = transform.position + (Vector3) Offset + rand;
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
