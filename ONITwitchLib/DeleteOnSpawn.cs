using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
///     Deletes an object immediately upon spawning.
/// </summary>
[PublicAPI]
public class DeleteOnSpawn : KMonoBehaviour
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	protected override void OnSpawn()
	{
		Destroy(gameObject);
	}
#pragma warning restore CS1591
}
