using ONITwitchCore.Content;

namespace ONITwitchCore.Cmps;

// component to add to all things that should be considered "floor tiles"
public class FloorTileExt : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Debug.Log($"adding {this} to floor tiles");
		ComponentsExt.FloorTiles.Add(this);
	}
	
	protected override void OnCleanUp()
	{
		Debug.Log($"removing {this} from floor tiles");
		ComponentsExt.FloorTiles.Remove(this);
		base.OnCleanUp();
	}
}
