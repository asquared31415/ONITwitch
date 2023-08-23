using System.Linq;
using ONITwitch.Content.Cmps;

namespace ONITwitch.Content;

internal static class ComponentsExt
{
	public static readonly Components.Cmps<OniTwitchFloorTileExt> FloorTiles = new();
	public static readonly Components.Cmps<OniTwitchToiletsExt> Toilets = new();
	public static readonly Components.Cmps<OniTwitchInsulatedTileExt> InsulatedTiles = new();

	public static void CollectFloorTiles()
	{
		foreach (var floor in Assets.BuildingDefs.Where(def => def.BuildingComplete.HasTag(GameTags.FloorTiles)))
		{
			floor.BuildingComplete.AddOrGet<OniTwitchFloorTileExt>();
		}
	}
}
