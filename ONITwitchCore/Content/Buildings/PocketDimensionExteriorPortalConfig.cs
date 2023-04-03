using JetBrains.Annotations;
using ONITwitch.Cmps.PocketDimension;
using ONITwitchLib;
using TUNING;
using UnityEngine;

namespace ONITwitch.Content.Buildings;

[UsedImplicitly]
internal class PocketDimensionExteriorPortalConfig : IBuildingConfig
{
	public const string Id = TwitchModInfo.ModPrefix + nameof(PocketDimensionExteriorPortalConfig);
	public const string Anim = "TI_PocketDim_kanim";

	public override BuildingDef CreateBuildingDef()
	{
		var def = BuildingTemplates.CreateBuildingDef(
			Id,
			1,
			2,
			Anim,
			100,
			60,
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER0,
			MATERIALS.REFINED_METALS,
			BUILDINGS.MELTING_POINT_KELVIN.TIER4,
			BuildLocationRule.OnFloor,
			DECOR.NONE,
			NOISE_POLLUTION.NONE
		);
		def.ShowInBuildMenu = false;
		def.Breakable = false;
		def.Entombable = false;
		def.Floodable = false;
		def.Invincible = true;
		def.PlayConstructionSounds = false;

		return def;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
	{
		// Portal component needs to be here to serialize
		go.AddOrGet<PocketDimensionExteriorPortal>();

		// make sure this doesn't count for building it outside the start biome
		go.AddTag(GameTags.TemplateBuilding);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<NavTeleporter>();

		var light = go.AddOrGet<Light2D>();
		light.Color = Color.white;
		light.overlayColour = LIGHT2D.LIGHT_OVERLAY;
		light.Range = 5f;
		light.Angle = 0.0f;
		light.Direction = LIGHT2D.DEFAULT_DIRECTION;
		light.Offset = new Vector2(0f, 1f);
		light.shape = LightShape.Circle;
		light.drawOverlay = false;
		light.Lux = 1800;

		var prefabID = go.GetComponent<KPrefabID>();
		prefabID.AddTag(GameTags.NoRocketRefund);

		go.AddOrGet<Deconstructable>().allowDeconstruction = false;

		go.AddOrGet<PocketDimensionExteriorPortalSideScreen>();
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
