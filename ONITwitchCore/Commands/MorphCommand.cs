using System.Linq;
using Klei.AI;
using ONITwitch.Toasts;
using UnityEngine;

namespace ONITwitch.Commands;

internal class MorphCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.Brains.Items.Any(static brain => brain is CreatureBrain);
	}

	public override void Run(object data)
	{
		foreach (var brain in Components.Brains.Items.Where(static brain => (brain != null) && brain is CreatureBrain))
		{
			DoMorph(brain.gameObject);
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.MORPH.TITLE, STRINGS.ONITWITCH.TOASTS.MORPH.BODY);
	}

	private static void DoMorph(GameObject orig)
	{
		// based on GravitasCreatureManipulator.Instance.SpawnMorph, but better
		// try to get a morph, first from the fertility monitor for adults, then go through the baby to get the adult if needed
		var eggTag = Tag.Invalid;
		var fertilitySmi = orig.GetSMI<FertilityMonitor.Instance>();
		if (fertilitySmi != null)
		{
			eggTag = FertilityMonitor.EggBreedingRoll(fertilitySmi.breedingChances, true);
		}
		else
		{
			var babySmi = orig.GetSMI<BabyMonitor.Instance>();
			if (babySmi != null)
			{
				var adult = Assets.TryGetPrefab(babySmi.def.adultPrefab);
				if (adult != null)
				{
					var adultFertilityDef = adult.GetDef<FertilityMonitor.Def>();
					if (adultFertilityDef != null)
					{
						eggTag = FertilityMonitor.EggBreedingRoll(adultFertilityDef.initialBreedingWeights, true);
					}
				}
			}
		}

		if (!eggTag.IsValid)
		{
			// no suitable morphs found or the required defs were not present
			return;
		}

		var egg = Assets.TryGetPrefab(eggTag);
		if (egg == null)
		{
			return;
		}

		var morphTag = egg.GetDef<IncubationMonitor.Def>().spawnedCreature;
		// if the parent was an adult, get the adult version
		if (fertilitySmi != null)
		{
			var babyGo = Assets.TryGetPrefab(morphTag);
			if (babyGo == null)
			{
				return;
			}

			morphTag = babyGo.GetDef<BabyMonitor.Def>().adultPrefab;
		}

		var position = orig.transform.GetPosition() with
		{
			z = Grid.GetLayerZ(Grid.SceneLayer.Creatures),
		};

		var go = Util.KInstantiate(Assets.GetPrefab(morphTag), position);
		go.SetActive(true);
		go.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim((HashedString) "growup_pst");
		foreach (var amount in orig.GetAmounts())
		{
			var amountInstance = amount.amount.Lookup(go);
			if (amountInstance != null)
			{
				var num = amount.value / amount.GetMax();
				amountInstance.value = num * amountInstance.GetMax();
			}
		}

		go.Trigger(-2027483228, orig);
		Object.Destroy(orig);
	}
}
