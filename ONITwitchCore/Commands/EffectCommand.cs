using Klei.AI;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class EffectCommand : CommandBase
{
	public override bool Condition(object data)
	{
		var effectId = (string) data;
		var effect = Db.Get().effects.TryGet(effectId);
		return (effect != null) && (Components.LiveMinionIdentities.Count > 0);
	}

	public override void Run(object data)
	{
		var effectId = (string) data;
		var effect = Db.Get().effects.TryGet(effectId);
		foreach (var minion in Components.LiveMinionIdentities.Items)
		{
			if (minion.TryGetComponent<Effects>(out var effects))
			{
				effects.Add(effect, true);
			}
		}

		ToastManager.InstantiateToast("Effect Applied", $"All dupes have had the {effect.Name} effect applied");
	}
}
