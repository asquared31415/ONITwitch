using System.Linq;
using ONITwitchCore.Cmps;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

internal class PoisonDupesCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	// ticks 8 times over half a cycle
	private const float DamageTime = Constants.SECONDS_PER_CYCLE / 2;
	private const int NumTicks = 8;

	public override void Run(object data)
	{
		foreach (var dot in Components.LiveMinionIdentities.Items.Select(
					 identity => identity.gameObject.AddOrGet<DamageOverTime>()
				 ))
		{
			dot.StartPoison(DamageTime, NumTicks);
			dot.enabled = true;
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.POISON.TITLE, STRINGS.ONITWITCH.TOASTS.POISON.BODY);
	}
}
