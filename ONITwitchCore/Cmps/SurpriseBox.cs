using UnityEngine;

namespace ONITwitchCore.Cmps;

public class SurpriseBox : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe((int) GameHashes.DeconstructComplete, OnDeconstruct);

		GetComponent<KBatchedAnimController>().Play("box", KAnim.PlayMode.Loop);
	}

	private void OnDeconstruct(object data)
	{
		for (var i = 0; i < 5; i++)
		{
			// spawn any random pickupable except:
			// dupes: they don't like being spawned like this
			// shockworm: unimplemented and crashy
			KPrefabID randPrefab;
			do
			{
				randPrefab = Assets.Prefabs.GetRandom();
			} while ((randPrefab.GetComponent<EnableSurpriseBoxMarker>() == null) &&
					 (
						 (randPrefab.GetComponent<Pickupable>() == null) ||
						 (randPrefab.GetComponent<MinionIdentity>() != null) ||
						 (randPrefab.PrefabID() == ShockwormConfig.ID)
					 ));

			Debug.Log($"[Twitch Integration] Surprise box selected prefab {randPrefab}");
			var go = Util.KInstantiate(randPrefab.gameObject, transform.position);
			go.SetActive(true);

			// make it fly a little bit
			var velocity = Random.Range(5, 10) * Random.insideUnitCircle.normalized;
			velocity.y = Mathf.Abs(velocity.y);
			if (GameComps.Fallers.Has(go))
			{
				GameComps.Fallers.Remove(go);
			}

			GameComps.Fallers.Add(go, velocity);

			GameScheduler.Instance.Schedule(
				"ONITwitch.RemoveSurpriseFaller",
				15f,
				_ =>
				{
					if (GameComps.Fallers.Has(go))
					{
						GameComps.Fallers.Remove(go);
					}
				}
			);
		}
	}
}
