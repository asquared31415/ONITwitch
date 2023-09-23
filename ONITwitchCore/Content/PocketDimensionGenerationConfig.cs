using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitch.Content.Cmps.PocketDimension;
using ONITwitchLib;
using ProcGen;
using UnityEngine;

// TODO: remove this once everything gets properly implemented
#pragma warning disable CS1591

namespace ONITwitch.Content;

/// <summary>
///     The base class for pocket dimension generation configs.
/// </summary>
[PublicAPI]
public abstract class BasePocketDimensionGeneration
{
	[CanBeNull] private readonly List<string> prefabIds;

	/// <summary>
	///     Instantiates a new generation config with no tile generation data.
	/// </summary>
	/// <param name="cyclesLifetime">The number of cycles for the dimension to stay open.</param>
	/// <param name="zoneType">The zone type of the dimension.</param>
	/// <param name="requiredSkillId">The required skill ID for this dimension to be considered for generation.</param>
	/// <param name="prefabIds">The list of prefabs that should spawn in this dimension.</param>
	[PublicAPI]
	protected BasePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null
	)
	{
		CyclesLifetime = cyclesLifetime;
		ZoneType = zoneType;
		RequiredSkillId = requiredSkillId;
		this.prefabIds = prefabIds;
	}

	internal float CyclesLifetime { get; }

	internal SubWorld.ZoneType ZoneType { get; }

	[CanBeNull] internal string RequiredSkillId { get; }

	protected abstract void GenerateTiles(WorldContainer world);

	protected virtual void GeneratePrefabs(WorldContainer world)
	{
		if (prefabIds != null)
		{
			// ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
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

	internal void Generate(WorldContainer world)
	{
		GenerateTiles(world);
		GeneratePrefabs(world);
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

public class TemplatePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		string template,
		[CanBeNull] string requiredSkillId = null,
		// ReSharper disable once ParameterTypeCanBeEnumerable.Local it can't
		[CanBeNull] List<string> prefabIds = null
	)
	: BasePocketDimensionGeneration(cyclesLifetime, zoneType, requiredSkillId, prefabIds)
{
	protected override void GenerateTiles(WorldContainer world)
	{
		var templateContainer = TemplateCache.GetTemplate(template);
		var pos = world.WorldOffset + PocketDimension.InternalOffset + PocketDimension.InternalSize / 2 - Vector2I.one;
		TemplateLoader.Stamp(templateContainer, pos, () => { });
	}
}

public class NoisePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[NotNull] IReadOnlyList<SimHashes> hashes,
		float xFrequency,
		float yFrequency,
		[CanBeNull] string requiredSkillId = null,
		// ReSharper disable once ParameterTypeCanBeEnumerable.Local it can't
		[CanBeNull] List<string> prefabIds = null
	)
	: BasePocketDimensionGeneration(cyclesLifetime, zoneType, requiredSkillId, prefabIds)
{
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
				// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags : the lower bits are flags
				var mass = (elements[idx].state & (Element.State) Element.StateMask) switch
				{
					Element.State.Vacuum => 0f,
					Element.State.Gas => 5f,
					Element.State.Liquid => 850f,
					Element.State.Solid => 2000f,
					_ => 0f,
				};
				SimMessages.ReplaceElement(
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
public class CustomPocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[NotNull] Action<WorldContainer> generateTilesAction,
		[CanBeNull] Action<WorldContainer> generatePrefabsAction,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null
	)
	: BasePocketDimensionGeneration(cyclesLifetime, zoneType, requiredSkillId, prefabIds)
{
	protected override void GenerateTiles(WorldContainer world)
	{
		generateTilesAction(world);
	}

	protected override void GeneratePrefabs(WorldContainer world)
	{
		if (generatePrefabsAction != null)
		{
			generatePrefabsAction(world);
		}
		else
		{
			base.GeneratePrefabs(world);
		}
	}
}
