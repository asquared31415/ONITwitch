using Klei.AI;
using ONITwitchCore.Content;

namespace ONITwitchCore.Commands;

public class SleepyDupesCommand : CommandBase
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

		/*ToastUiManager.InstantiateToast("Sleepy Dupes", "All of your dupes have become extremely exhausted");*/
	}
}
