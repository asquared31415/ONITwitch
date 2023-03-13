using ONITwitchCore.Content;

namespace ONITwitchCore.Cmps;

internal class ToiletsExt : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComponentsExt.Toilets.Add(this);
	}

	protected override void OnCleanUp()
	{
		ComponentsExt.Toilets.Remove(this);
		base.OnCleanUp();
	}
}
