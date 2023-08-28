using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONITwitch.Content.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
internal class OniTwitchRainPrefab : KMonoBehaviour
{
	private float accumTime;
	private int countRemaining;
	private List<(Tag Tag, float Weight)> prefabChances;
	private float timePerItem;

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

				var prefabTag = GetRandomTag();
				if (!prefabTag.IsValid)
				{
					continue;
				}

				var prefab = Assets.GetPrefab(prefabTag);
				if (prefab == null)
				{
					continue;
				}

				var go = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Move);
				var kPrefabID = go.GetComponent<KPrefabID>();

				if ((go.GetComponent<ElementChunk>() != null) && go.TryGetComponent<PrimaryElement>(out var element))
				{
					element.Mass = 10;
					var gridTemp = Grid.Temperature[cellWithClearance];
					element.Temperature = gridTemp > 0 ? gridTemp : element.Element.defaultValues.temperature;
				}

				kPrefabID.InitializeTags();

				// Put prefab in the center of a cell, on the front-most layer.
				// It will move backwards on the next layer update.
				go.transform.SetPosition(Grid.CellToPosCCC(cellWithClearance, Grid.SceneLayer.Move));

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

	public void Initialize(float time, int count, List<(Tag Tag, float Weight)> ids)
	{
		timePerItem = time;
		countRemaining = count;
		prefabChances = ids;

		if (prefabChances.Count == 0)
		{
			// this is a warning for mod devs to see, even if they're working against the release version
			Log.Warn("Cannot rain list of zero prefabs");
			Log.Warn(Environment.StackTrace);

			enabled = false;
			return;
		}

		// the component may have been disabled previously, enable it for this rain
		enabled = true;
	}

	private Tag GetRandomTag()
	{
		var sum = prefabChances.Sum(pair => pair.Weight);
		var rand = Random.value * sum;
		foreach (var (prefabTag, weight) in prefabChances)
		{
			rand -= weight;
			if (rand <= 0)
			{
				return prefabTag;
			}
		}

		Log.Warn("Unable to select a random prefab for prefab rain");
		foreach (var (prefabTag, weight) in prefabChances)
		{
			Log.Debug($"{prefabTag}: {weight}");
		}

		return Tag.Invalid;
	}
}
