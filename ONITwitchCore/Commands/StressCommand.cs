using System.Linq;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

internal class StressCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		var stressPercent = (double) data;

		var stress = Db.Get().Amounts.Stress;
		foreach (var s in Components.LiveMinionIdentities.Items.Select(i => stress.Lookup(i.gameObject))
					 .Where(s => s != null))
		{
			s.ApplyDelta((float) stressPercent * 100);
		}

		if (stressPercent >= 0)
		{
			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.STRESS_INCREASE.TITLE, STRINGS.ONITWITCH.TOASTS.STRESS_INCREASE.BODY);
		}
		else
		{
			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.STRESS_DECREASE.TITLE, STRINGS.ONITWITCH.TOASTS.STRESS_DECREASE.BODY);
		}
	}
}
