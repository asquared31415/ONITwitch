using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class PeeCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		var peeUrge = Db.Get().Urges.Pee;
		foreach (var identity in Components.LiveMinionIdentities.Items)
		{
			if (identity.TryGetComponent<ChoreConsumer>(out var consumer) &&
				identity.TryGetComponent<ChoreProvider>(out var provider))
			{
				consumer.AddUrge(peeUrge);
				provider.AddChore(new PeeChore(identity.gameObject.GetComponent<StateMachineController>()));
			}
		}

		ToastManager.InstantiateToast(
			"Pee Everywhere",
			"All of your duplicants suddenly had the urge to relieve themselves"
		);
	}
}
