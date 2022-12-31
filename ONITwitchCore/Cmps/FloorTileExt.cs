using ONITwitchCore.Content;

namespace ONITwitchCore.Cmps;

// component to add to all things that should be considered "floor tiles"
public class FloorTileExt : KMonoBehaviour
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
