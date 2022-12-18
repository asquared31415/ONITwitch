using ONITwitchCore.Cmps;
using TUNING;
using UnityEngine;

namespace ONITwitchCore.Content.Buildings;

public class SurpriseBoxConfig : IBuildingConfig
{
	public const string Id = "ONITwitch." + nameof(SurpriseBoxConfig);
	public const string Anim = "twitch_surprise_box_kanim";

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
		def.ThermalConductivity = 0;

		return def;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<SurpriseBox>();
		go.AddOrGet<Deconstructable>();
	}
}
