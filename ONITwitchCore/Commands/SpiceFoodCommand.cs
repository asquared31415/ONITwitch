using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;

namespace ONITwitch.Commands;

internal class SpiceFoodCommand : CommandBase
{
	private static readonly AccessTools.FieldRef<Edible, List<SpiceInstance>> SpicesAccessor =
		AccessTools.FieldRefAccess<Edible, List<SpiceInstance>>("spices");

	public override void Run(object data)
	{
		var validSpices = GetValidSpiceIds();
		if (validSpices.Count == 0)
		{
			Log.Warn("No spices were enabled for the spice event");
			return;
		}

		var status = Db.Get().MiscStatusItems.SpicedFood;

		// add all spices to this food (unless they already exist) 
		foreach (var item in Components.Edibles.Items)
		{
			var existingSpices = SpicesAccessor(item).Select(spice => spice.Id.Name);
			foreach (var add in validSpices.Except(existingSpices))
			{
				item.SpiceEdible(new SpiceInstance { Id = add }, status);
			}
		}

		ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.SPICE_FOOD.TITLE, STRINGS.ONITWITCH.TOASTS.SPICE_FOOD.BODY);
	}

	private static List<string> GetValidSpiceIds()
	{
		return Db.Get()
			.Spices.resources.Where(spice => DlcManager.IsDlcListValidForCurrentContent(spice.DlcIds))
			.Select(spice => spice.Id)
			.ToList();
	}
}
