using System.Collections.Generic;
using JetBrains.Annotations;
using KSerialization;
using ONITwitchCore.Content.Buildings;
using ONITwitchDefaultCommands.PocketDimension;

namespace ONITwitchCore.Content.Entities;

[SerializationConfig(MemberSerialization.OptIn)]
public class PocketDimensionClusterGridEntity : ClusterGridEntity
{
	public override string Name => "Pocket Dimension";
	public override EntityLayer Layer => EntityLayer.POI;

	[CanBeNull] private List<AnimConfig> animConfigs;

	// Used in the *default* GetUISprite, only the first entry is used, and only its anim file
	// The anim must have a `ui` anim name with a `ui` symbol at frame 0
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
