using JetBrains.Annotations;
using Klei.AI;
using ONITwitchCore.Cmps;
using ONITwitchLib;
using TUNING;
using UnityEngine;

namespace ONITwitchCore.Content.Entities;

[UsedImplicitly]
internal class GlitterPuftConfig : IEntityConfig
{
	// suboptimal, but required for save compat, might be able to fix to "ONITwitch." later
	public const string Id = "TI." + nameof(GlitterPuftConfig);
	
	private const string BaseTraitId = "TI." + "GlitterPuftBaseTrait";
	private const string Anim = "TI_GlitterPuft_kanim";

	private static readonly Tag SpeciesTag = GameTags.Creatures.Species.PuftSpecies;

	private static void CreateBaseTrait()
	{
		var trait = Db.Get()
			.CreateTrait(
				BaseTraitId,
				STRINGS.CREATURES.SPECIES.ONITWITCH.GLITTERPUFT.NAME,
				STRINGS.CREATURES.SPECIES.ONITWITCH.GLITTERPUFT.NAME,
				null,
				false,
				null,
				true,
				true
			);
		trait.Add(
			new AttributeModifier(
				Db.Get().Amounts.HitPoints.maxAttribute.Id,
				float.PositiveInfinity,
				STRINGS.CREATURES.SPECIES.ONITWITCH.GLITTERPUFT.NAME
			)
		);
	}

	public GameObject CreatePrefab()
	{
		const string initialAnim = "idle_loop";

		var go = EntityTemplates.CreatePlacedEntity(
			Id,
			STRINGS.CREATURES.SPECIES.ONITWITCH.GLITTERPUFT.NAME,
			STRINGS.CREATURES.SPECIES.ONITWITCH.GLITTERPUFT.DESC,
			5f,
			Assets.GetAnim(Anim),
			initialAnim,
			Grid.SceneLayer.Creatures,
			1,
			1,
			new EffectorValues
			{
				amount = 500,
				radius = 7,
			},
			NOISE_POLLUTION.NONE
		);

		const string navGrid = "FlyerNavGrid1x1";
		const float lowTemp = 0f;
		const float highTemp = 10_000f;

		CreateBaseTrait();

		EntityTemplates.ExtendEntityToBasicCreature(
			go,
			FactionManager.FactionID.Friendly,
			BaseTraitId,
			navGrid,
			NavType.Hover,
			onDeathDropCount: 0,
			drownVulnerable: false,
			entombVulnerable: false,
			warningLowTemperature: lowTemp,
			warningHighTemperature: highTemp,
			lethalLowTemperature: lowTemp,
			lethalHighTemperature: highTemp
		);
		go.AddOrGet<FactionAlignment>().canBePlayerTargeted = false;
		Object.Destroy(go.GetComponent<RangedAttackable>());

		var component = go.AddOrGet<KPrefabID>();
		component.AddTag(GameTags.Creatures.Flyer);

		go.AddOrGetDef<SubmergedMonitor.Def>();
		go.AddOrGet<LoopingSounds>();

		go.AddOrGetDef<LureableMonitor.Def>().lures = new[]
		{
			GameTags.SlimeMold,
		};

		// It really just kinda has to float around and sleep
		var choreTable = new ChoreTable.Builder().Add(new AnimInterruptStates.Def())
			.Add(new CreatureSleepStates.Def())
			.Add(new MoveToLureStates.Def())
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(go, choreTable, SpeciesTag, null);

		var light = go.AddOrGet<Light2D>();
		light.Color = Color.white;
		light.overlayColour = LIGHT2D.LIGHT_OVERLAY;
		light.Range = 5f;
		light.Angle = 0.0f;
		light.Direction = LIGHT2D.DEFAULT_DIRECTION;
		light.Offset = new Vector2(0f, 0.5f);
		light.shape = LightShape.Circle;
		light.drawOverlay = true;
		light.Lux = 1800;
		go.AddOrGet<LightSymbolTracker>().targetSymbol = (HashedString) "body";

		go.AddOrGet<GlitterPuft>();
		const int numSparkles = 2;
		go.AddOrGet<SparkleFollower>().NumSparkles = numSparkles;

		return go;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		var tracker = new GameObject("Glitter Puft Tracker");
		tracker.AddComponent<GlitterPuftTracker>();
		tracker.transform.SetParent(inst.transform, false);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}
}
