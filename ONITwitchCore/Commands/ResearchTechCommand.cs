using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;

namespace ONITwitch.Commands;

internal class ResearchTechCommand : CommandBase
{
	private static readonly string[] AllowedResearchKinds = { "basic", "advanced", "nuclear" };

	public override bool Condition(object data)
	{
		return GetAllowedTechs().Count > 0;
	}

	public override void Run(object data)
	{
		var possibleTechs = GetAllowedTechs();
		if (possibleTechs.Count == 0)
		{
			Log.Warn("Cannot find a Tech to research");
			return;
		}

		// only research a research that is of the minimum tier that has not been complete
		var minTier = possibleTechs.Min(static tech => tech.tier);

		var minTechList = possibleTechs.Where(tech => tech.tier == minTier).ToList();

		if (minTechList.Count > 0)
		{
			var tech = minTechList.GetRandom();
			var techInstance = Research.Instance.GetOrAdd(tech);
			techInstance.Purchased();
			Game.Instance.Trigger((int) GameHashes.ResearchComplete, tech);

			var techName = Util.StripTextFormatting(
				Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".NAME").ToString()
			);

			ToastManager.InstantiateToast(
				STRINGS.ONITWITCH.TOASTS.RESEARCH_TECH.TITLE,
				string.Format(STRINGS.ONITWITCH.TOASTS.RESEARCH_TECH.BODY_FORMAT, techName)
			);
		}
		else
		{
			Log.Info(
				"Tech command could not find any techs to research, it was probably done between condition and run"
			);
		}
	}

	[NotNull]
	private static List<Tech> GetAllowedTechs()
	{
		var techs = Db.Get().Techs;
		// only add techs that have 0 (or missing) costs for space science (or any modded kinds) 
		// and that have their prereqs satisfied and that are not complete
		return techs.resources.Where(
				static tech => tech.costsByResearchTypeID.Keys.All(
					researchKind =>
						AllowedResearchKinds.Contains(researchKind) || (tech.costsByResearchTypeID[researchKind] == 0)
				) && tech.ArePrerequisitesComplete() && !tech.IsComplete()
			)
			.ToList();
	}
}
