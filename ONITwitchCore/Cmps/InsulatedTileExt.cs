using ONITwitchCore.Content;

namespace ONITwitchCore.Cmps;

internal class InsulatedTileExt : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComponentsExt.InsulatedTiles.Add(this);
	}

	protected override void OnCleanUp()
	{
		ComponentsExt.InsulatedTiles.Remove(this);
		base.OnCleanUp();
	}
}
