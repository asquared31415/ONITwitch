namespace ONITwitch.Content.Cmps;

internal class OniTwitchToiletsExt : KMonoBehaviour
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
