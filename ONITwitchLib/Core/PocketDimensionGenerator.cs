using System;
using ONITwitchLib.Attributes;

namespace ONITwitchLib.Core;

#if DEBUG // don't compile this into release builds
/// <summary>
///     Provides methods for adding new pocket dimensions to the generation pool and to generate pocket dimensions.
/// </summary>
[NotPublicAPI] // make this public API once it's implemented
[Obsolete(
	"This is not yet implemented. Eventually you will be able to add custom pocket dimensions or generate them manually.",
	true
)]
public static class PocketDimensionGenerator
{
}
#endif
