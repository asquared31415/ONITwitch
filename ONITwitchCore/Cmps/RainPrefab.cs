using System.Collections.Generic;
using System.Text;
using KSerialization;
using ONITwitchLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class RainPrefab : KMonoBehaviour
{
	[Serialize] [SerializeField] private float timePerItem;
	[Serialize] [SerializeField] private int countRemaining;
	[Serialize] [SerializeField] private List<string> prefabIds;

	private float accumTime;
	private readonly List<GameObject> prefabs = new();

	public void Initialize(float time, int count, List<string> ids)
	{
		timePerItem = time;
		countRemaining = count;
		prefabIds = ids;

		prefabs.Clear();

		foreach (var prefabId in prefabIds)
		{
			var prefab = Assets.TryGetPrefab(prefabId);
			if (prefab != null)
			{
				prefabs.Add(prefab);
			}
		}

		if (prefabs.Count == 0)
		{
			var sb = new StringBuilder();
			foreach (var prefabId in prefabIds)
			{
				sb.Append($"{prefabId} ");
			}

			Debug.LogWarning($"[Twitch Integration] Unable to find prefab(s) {sb}for rain");
			enabled = false;
		}
	}

	private void Update()
	{
		if (countRemaining > 0)
		{
			accumTime += Time.deltaTime;

			var rainCount = Mathf.FloorToInt(accumTime / timePerItem);
			for (var i = 0; i < rainCount; i++)
			{
				var min = PosUtil.CameraMinWorldPos();
				var max = PosUtil.CameraMaxWorldPos();
				var rainMin = min + new Vector3((max.x - min.x) * 0.2f, (max.y - min.y) * 0.6f);
				var rainMax = max + new Vector3(-(max.x - min.x) * 0.2f, -(max.y - min.y) * 0.2f);

				var randPosCell = Grid.XYToCell(
					(int) Random.Range(rainMin.x, rainMax.x),
					(int) Random.Range(rainMin.y, rainMax.y)
				);
				var cellWithClearance = GridUtil.NearestEmptyCell(randPosCell);

				var prefab = prefabs.GetRandom();
				var go = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore);

				var sceneLayer = Grid.SceneLayer.Front;
				if (go.TryGetComponent<KBatchedAnimController>(out var kbac))
				{
					sceneLayer = kbac.sceneLayer;
				}

				var pos = Grid.CellToPosCCC(cellWithClearance, sceneLayer);
				var kPrefabID = go.GetComponent<KPrefabID>();

				if ((go.GetComponent<ElementChunk>() != null) && go.TryGetComponent<PrimaryElement>(out var element))
				{
					element.Mass = 10;
					element.Temperature = Grid.Temperature[Grid.PosToCell(pos)];
				}

				kPrefabID.InitializeTags();


				go.transform.SetPosition(pos);

				// Prefab specific post-initialization
				switch (kPrefabID.tag)
				{
					case BeeConfig.ID:
					{
						// Remove temperature limits for bees spawned via rain
						if (go.TryGetComponent<TemperatureVulnerable>(out var temperatureVulnerable))
						{
							temperatureVulnerable.Configure(0, 0, 10_000, 10_000);
						}

						break;
					}
				}

				go.SetActive(true);
			}

			countRemaining -= rainCount;
			accumTime %= timePerItem;
		}
		else
		{
			enabled = false;
		}
	}
}
