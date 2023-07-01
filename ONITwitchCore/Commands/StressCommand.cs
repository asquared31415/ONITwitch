using System.Linq;
using ONITwitch.Toasts;

namespace ONITwitch.Commands;

internal class StressCommand : CommandBase
{
	public override bool Condition(object data)
	{
		var stress = Db.Get().Amounts.Stress;
		var isStressUp = (double) data > 0;
		return Components.LiveMinionIdentities.Any(
			dupe =>
			{
				var stressVal = stress.Lookup(dupe).value;
				// always let stress up work, but only do stress down if there's a stressed dupe
				if (isStressUp)
				{
					return true;
				}

				return stressVal > 50;
			}
		);
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
			ToastManager.InstantiateToast(
				STRINGS.ONITWITCH.TOASTS.STRESS_INCREASE.TITLE,
				STRINGS.ONITWITCH.TOASTS.STRESS_INCREASE.BODY
			);
		}
		else
		{
			ToastManager.InstantiateToast(
				STRINGS.ONITWITCH.TOASTS.STRESS_DECREASE.TITLE,
				STRINGS.ONITWITCH.TOASTS.STRESS_DECREASE.BODY
			);
		}
	}
}
