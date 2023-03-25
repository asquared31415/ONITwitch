using HarmonyLib;
using ONITwitch.Toasts;
using UnityEngine;

namespace ONITwitch.Commands;

internal class FartCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		foreach (var minion in Components.LiveMinionIdentities.Items)
		{
			DoFart(minion.gameObject, (float) (double) data);
		}

		DoCringeEffect();

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.FART.TITLE, STRINGS.ONITWITCH.TOASTS.FART.BODY);
	}

	private static readonly AccessTools.FieldRef<HashedString[]> WorkAnimsGetter =
		AccessTools.StaticFieldRefAccess<HashedString[]>(AccessTools.Field(typeof(Flatulence), "WorkLoopAnims"));

	private static void DoFart(GameObject dupe, float fartMass)
	{
		// most of this logic copied from `Flatulence`
		var dupePos = dupe.transform.position;
		var temperature = Db.Get().Amounts.Temperature.Lookup(dupe).value;
		var equippable = dupe.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
		if (equippable != null)
		{
			equippable.GetComponent<Storage>()
				.AddGasChunk(SimHashes.Methane, fartMass, temperature, byte.MaxValue, 0, false);
		}
		else
		{
			SimMessages.AddRemoveSubstance(
				Grid.PosToCell(dupePos),
				SimHashes.Methane,
				CellEventLogger.Instance.ElementConsumerSimUpdate,
				fartMass,
				temperature,
				byte.MaxValue,
				0
			);
			var effect = FXHelpers.CreateEffect(
				"odor_fx_kanim",
				dupePos,
				dupe.transform,
				true
			);
			effect.Play(WorkAnimsGetter());
			effect.destroyOnAnimComplete = true;
		}

		var objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(dupe);
		var audioPos = dupePos with { z = 0.0f };
		var volume = 1f;
		if (objectIsSelectedAndVisible)
		{
			audioPos = SoundEvent.AudioHighlightListenerPosition(audioPos);
			volume = SoundEvent.GetVolume(true);
		}

		KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), audioPos, volume);
	}

	private static void DoCringeEffect()
	{
		foreach (var minionIdentity in Components.LiveMinionIdentities.Items)
		{
			minionIdentity.Trigger(
				(int) GameHashes.Cringe,
				Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String
			);
			minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
		}
	}
}
