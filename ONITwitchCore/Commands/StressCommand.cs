using System.Linq;

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

		/*
		if (stressPercent >= 0)
		{
			ToastUiManager.InstantiateToast(
				"Stress Wave",
				"All duplicants have become suddenly stressed about life in the colony"
			);
		}
		else
		{
			ToastUiManager.InstantiateToast(
				"Calming Wave",
				"All duplicants have been calmed about the stressful situation which is life in an asteroid"
			);
		}
		*/
	}
}
