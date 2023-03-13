using System.Collections.Generic;
using KMod;

namespace ONITwitchCore.Integration;

internal class ModIntegration
{
	private readonly Dictionary<string, Mod> mods = new();

	public ModIntegration(IEnumerable<Mod> mods)
	{
		foreach (var mod in mods)
		{
			this.mods[mod.staticID] = mod;
		}
	}

	public Mod GetModIfActive(string staticID)
	{
		return mods.TryGetValue(staticID, out var mod) && mod.IsEnabledForActiveDlc() ? mod : null;
	}

	public bool IsModPresentAndActive(string staticID)
	{
		return GetModIfActive(staticID) != null;
	}
}
