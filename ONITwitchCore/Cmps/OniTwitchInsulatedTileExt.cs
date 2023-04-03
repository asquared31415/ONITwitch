using ONITwitch.Content;

namespace ONITwitch.Cmps;

internal class OniTwitchInsulatedTileExt : KMonoBehaviour
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
