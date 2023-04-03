using ONITwitch.Content;

namespace ONITwitch.Cmps;

/// <summary>
/// Component to add to all things that should be considered "floor tiles". 
/// </summary>
internal class OniTwitchFloorTileExt : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComponentsExt.FloorTiles.Add(this);
	}
	
	protected override void OnCleanUp()
	{
		ComponentsExt.FloorTiles.Remove(this);
		base.OnCleanUp();
	}
}
