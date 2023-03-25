using Klei.AI;
using ONITwitch.Content;
using ONITwitch.Toasts;

namespace ONITwitch.Commands;

// TODO: this is EffectCommand but with a different toast, can this be merged?
internal class SleepyDupesCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		foreach (var identity in Components.LiveMinionIdentities.Items)
		{
			if (identity.TryGetComponent<Effects>(out var effects))
			{
				effects.Add(CustomEffects.SleepyEffect, true);
			}
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.SLEEPY_DUPES.TITLE, STRINGS.ONITWITCH.TOASTS.SLEEPY_DUPES.BODY);
	}
}
