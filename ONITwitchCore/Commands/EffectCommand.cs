using Klei.AI;

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
		var minion = Components.LiveMinionIdentities.Items.GetRandom();
		if (minion != null)
		{
			if (minion.TryGetComponent<Effects>(out var effects))
			{
				var effectId = (string) data;
				var effect = Db.Get().effects.TryGet(effectId);
				effects.Add(effect, true);
			}
		}
	}
}
