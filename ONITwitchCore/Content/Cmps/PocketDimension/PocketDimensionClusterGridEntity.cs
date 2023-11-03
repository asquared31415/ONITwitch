using System.Collections.Generic;
using JetBrains.Annotations;
using KSerialization;
using ONITwitch.Content.BuildingConfigs;

namespace ONITwitch.Content.Cmps.PocketDimension;

[SerializationConfig(MemberSerialization.OptIn)]
internal class PocketDimensionClusterGridEntity : ClusterGridEntity
{
	[CanBeNull] private List<AnimConfig> animConfigs;

	public override string Name => STRINGS.ONITWITCH.WORLDS.POCKET_DIMENSION.NAME;
	public override EntityLayer Layer => EntityLayer.POI;

	// Used in the *default* GetUISprite, only the first entry is used, and only its anim file
	// The anim must have a `ui` anim name with a `ui` symbol at frame 0
	[NotNull]
	public override List<AnimConfig> AnimConfigs
	{
		get
		{
			animConfigs ??= new List<AnimConfig>
			{
				new()
				{
					animFile = Assets.GetAnim(PocketDimensionExteriorPortalConfig.Anim),
				},
			};

			return animConfigs;
		}
	}

	public override bool IsVisible => false;
	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Hidden;
}
