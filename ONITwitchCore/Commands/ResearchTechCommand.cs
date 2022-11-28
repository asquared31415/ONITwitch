using System.Collections.Generic;
using System.Linq;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class ResearchTechCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return GetAllowedTechs().Count > 0;
	}

	public override void Run(object data)
	{
		var possibleTechs = GetAllowedTechs();

		// only research a research that is of the minimum tier that has not been complete
		var minTier = possibleTechs.Min(tech => tech.tier);

		var minTechList = possibleTechs.Where(tech => tech.tier == minTier).ToList();

		if (minTechList.Count > 0)
		{
			var tech = minTechList.GetRandom();
			var techInstance = Research.Instance.GetOrAdd(tech);
			techInstance.Purchased();
			Game.Instance.Trigger((int) GameHashes.ResearchComplete, tech);

			var techName = Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".NAME").ToString();
			ToastManager.InstantiateToast("Innovation Nation", $"{techName} has been researched");
		}
	}
	
	private static readonly string[] AllowedTechs = { "basic", "advanced", "nuclear" };

	private static List<Tech> GetAllowedTechs()
	{
		var techs = Db.Get().Techs;
		// only add techs that don't have any costs except the first two tiers (and in SO, nuclear science)
		// and that have their prereqs satisfied and that are not complete
		return techs.resources.Where(
				tech => !tech.costsByResearchTypeID.Keys.Except(AllowedTechs).Any() &&
						tech.ArePrerequisitesComplete() &&
						!tech.IsComplete()
			)
			.ToList();
	}
}
