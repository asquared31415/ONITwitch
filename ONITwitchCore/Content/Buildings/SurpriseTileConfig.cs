using ONITwitchCore.Cmps;
using TUNING;
using UnityEngine;

namespace ONITwitchCore.Content.Buildings;

public class SurpriseTileConfig : IBuildingConfig
{
	public const string Id = "ONITwitch." + nameof(SurpriseTileConfig);
	public const string Anim = "floor_basic_kanim";

	public override BuildingDef CreateBuildingDef()
	{
		var def = BuildingTemplates.CreateBuildingDef(
			Id,
			1,
			1,
			Anim,
			10,
			10,
			new[] { 100f },
			MATERIALS.ALL_METALS,
			2400,
			BuildLocationRule.Anywhere,
			DECOR.NONE,
			NOISE_POLLUTION.NONE
		);
		def.Floodable = false;
		def.Overheatable = false;
		def.PlayConstructionSounds = false;
		def.DebugOnly = true;

		return def;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<SurpriseTile>();
		go.AddOrGet<Deconstructable>();
	}
}
