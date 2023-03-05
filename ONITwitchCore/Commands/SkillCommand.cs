using System.Linq;
using ONITwitchLib;
using UnityEngine;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class SkillCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		var skillFraction = (double) data;
		// copy for easier choice
		var minions = Components.LiveMinionIdentities.Items.ToList();
		// if skillFraction is 0.0 or there are 0 minions, this will be zero and not try to pick a minion
		var skillCount = Mathf.CeilToInt((float) skillFraction * minions.Count);

		minions.ShuffleList();
		for (var idx = 0; idx < skillCount; idx++)
		{
			if (minions[idx].TryGetComponent<MinionResume>(out var resume))
			{
				resume.ForceAddSkillPoint();
			}
		}

		ToastManager.InstantiateToast(STRINGS.TOASTS.SKILLS.TITLE, STRINGS.TOASTS.SKILLS.BODY);
	}
}
