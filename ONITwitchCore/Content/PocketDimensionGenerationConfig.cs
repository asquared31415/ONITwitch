using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchCore.Cmps.PocketDimension;
using ONITwitchCore.Content.Entities;
using ONITwitchLib;
using ProcGen;
using UnityEngine;

namespace ONITwitchCore.Content;

public abstract class BasePocketDimensionGeneration
{
	public float CyclesLifetime;
	public SubWorld.ZoneType ZoneType;

	[CanBeNull] public string RequiredSkillId;

	[CanBeNull] private readonly List<string> prefabIds;

	private readonly bool canSpawnSubDimensions;

	// TODO: enable once sub-dimensions are less totally broken
	private const bool DEBUG_BROKEN_AllowNestedDimensions = false;

	public BasePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	)
	{
		CyclesLifetime = cyclesLifetime;
		ZoneType = zoneType;
		RequiredSkillId = requiredSkillId;
		this.prefabIds = prefabIds;
		// TODO: enable once sub-dimensions are less totally broken
		this.canSpawnSubDimensions = DEBUG_BROKEN_AllowNestedDimensions & canSpawnSubDimensions;
	}

	public void Generate(WorldContainer world)
	{
		GenerateTiles(world);
		GeneratePrefabs(world);

		// 5% chance to spawn another pocket dimension inside
		if (canSpawnSubDimensions && (ThreadRandom.Next(20) == 0))
		{
			GenerateSubDimension(world);
		}
	}

	protected abstract void GenerateTiles(WorldContainer world);

	private void GeneratePrefabs(WorldContainer world)
	{
		if (prefabIds != null)
		{
			foreach (var prefabId in prefabIds)
			{
				var prefab = Assets.GetPrefab(prefabId);
				if (prefab != null)
				{
					var pos = world.WorldOffset + GetRandomInteriorOffset();
					var go = Util.KInstantiate(prefab, (Vector2) pos);
					go.SetActive(true);
				}
			}
		}
	}

	private static void GenerateSubDimension(WorldContainer world)
	{
		var pos = world.WorldOffset + GetRandomInteriorOffset();
		var go = Util.KInstantiate(Assets.GetPrefab(DevPocketDimensionGeneratorConfig.Id), (Vector2) pos);
		go.SetActive(true);

		var pocketDim = world.GetComponent<PocketDimension>();
		// if a dimension was spawned inside, increase the lifetime by 3 more cycles
		// TODO: tune this 3 cycles, and adjust comment above
		// TODO: is there a way to get the children to spawn fully first, and wait for that, to add their lifetime
		pocketDim.Lifetime += 3 * Constants.SECONDS_PER_CYCLE;
		pocketDim.MaxLifetime += 3 * Constants.SECONDS_PER_CYCLE;
	}

	private static Vector2I GetRandomInteriorOffset()
	{
		var randomX = ThreadRandom.Next(
			PocketDimension.InternalOffset.x + 1,
			PocketDimension.InternalOffset.x +
			PocketDimension.InternalSize.x - 1
		);
		var randomY = ThreadRandom.Next(
			PocketDimension.InternalOffset.y + 1,
			PocketDimension.InternalOffset.y +
			PocketDimension.InternalSize.y - 1
		);

		return new Vector2I(randomX, randomY);
	}
}

public class TemplatePocketDimensionGeneration : BasePocketDimensionGeneration
{
	private readonly string template;

	public TemplatePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		string template,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	) : base(cyclesLifetime, zoneType, requiredSkillId, prefabIds, canSpawnSubDimensions)
	{
		this.template = template;
	}

	protected override void GenerateTiles(WorldContainer world)
	{
		var templateContainer = TemplateCache.GetTemplate(template);
		var pos = world.WorldOffset + PocketDimension.InternalOffset + PocketDimension.InternalSize / 2 - Vector2I.one;
		TemplateLoader.Stamp(templateContainer, pos, () => { });
	}
}

public class NoisePocketDimensionGeneration : BasePocketDimensionGeneration
{
	[NotNull] private readonly List<SimHashes> hashes;

	private readonly float xFrequency;
	private readonly float yFrequency;

	public NoisePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[NotNull] List<SimHashes> hashes,
		float xFrequency,
		float yFrequency,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	) : base(cyclesLifetime, zoneType, requiredSkillId, prefabIds, canSpawnSubDimensions)
	{
		this.hashes = hashes;
		this.xFrequency = xFrequency;
		this.yFrequency = yFrequency;
	}

	protected override void GenerateTiles(WorldContainer world)
	{
		var seed = Time.realtimeSinceStartup;

		var numElements = hashes.Count;
		var elements = hashes.Select(ElementLoader.FindElementByHash).ToList();

		for (var x = 0; x < PocketDimension.InternalSize.x; x++)
		{
			var xPos = world.WorldOffset.x + PocketDimension.InternalOffset.x + x;
			for (var y = 0; y < PocketDimension.InternalSize.y; y++)
			{
				var yPos = world.WorldOffset.y + PocketDimension.InternalOffset.y + y;
				// Transform from (-1,1) to (0,1)
				var val = (PerlinSimplexNoise.noise(xPos * xFrequency, yPos * yFrequency, seed) + 1) / 2;
				var idx = (int) Mathf.Floor(val * numElements);
				var defaultValues = elements[idx].defaultValues;
				var mass = (elements[idx].state & (Element.State) Element.StateMask) switch
				{
					Element.State.Vacuum => 0f,
					Element.State.Gas => 5f,
					Element.State.Liquid => 850f,
					Element.State.Solid => 2000f,
					_ => 0f,
				};
				SimMessages.ReplaceAndDisplaceElement(
					Grid.XYToCell(xPos, yPos),
					hashes[idx],
					null,
					mass,
					defaultValues.temperature
				);
			}
		}
	}
}

// Delegates the call to GenerateTiles to an action passed
// Used for mod compatibility via reflection
[UsedImplicitly]
public class CustomPocketDimensionGeneration : BasePocketDimensionGeneration
{
	private readonly Action<WorldContainer> generateTilesAction;

	public CustomPocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		Action<WorldContainer> generateTilesAction,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	) : base(cyclesLifetime, zoneType, requiredSkillId, prefabIds, canSpawnSubDimensions)
	{
		this.generateTilesAction = generateTilesAction;
	}

	protected override void GenerateTiles(WorldContainer world)
	{
		generateTilesAction(world);
	}
}
