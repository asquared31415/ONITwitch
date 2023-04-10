using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// Additional tags that the Twitch mod uses
/// </summary>
[PublicAPI]
public static class ExtraTags
{
	/// <summary>
	/// A tag that explicitly enables prefabs to be spawned by the Surprise Box, even if it would not by default.
	/// </summary>
	[PublicAPI]
	public static Tag OniTwitchSurpriseBoxForceEnabled = nameof(OniTwitchSurpriseBoxForceEnabled);

	/// <summary>
	/// A tag that explicitly stops prefabs from being spawned by the Surprise Box, even if the prefab could by default.
	/// </summary>
	[PublicAPI]
	public static Tag OniTwitchSurpriseBoxForceDisabled = nameof(OniTwitchSurpriseBoxForceDisabled);
}
