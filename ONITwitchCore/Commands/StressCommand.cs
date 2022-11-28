using System.Linq;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class StressCommand : CommandBase
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
			ToastManager.InstantiateToast(
				"Stress Wave",
				"All duplicants have become suddenly stressed about life in the colony"
			);
		}
		else
		{
			ToastManager.InstantiateToast(
				"Calming Wave",
				"All duplicants have been calmed about the stressful situation which is life in an asteroid"
			);
		}
	}
}
