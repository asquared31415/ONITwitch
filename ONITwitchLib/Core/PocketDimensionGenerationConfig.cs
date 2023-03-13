using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ProcGen;

namespace ONITwitchLib.Core;

/// <summary>
/// The base class for pocket dimension generation configs. You should not use this.
/// Extending it will not work properly, as none of the types you create will extend the base mod.
/// Use <see cref="CustomPocketDimensionGeneration"/> to do custom dimension generation.
/// </summary>
[PublicAPI]
public abstract class BasePocketDimensionGeneration
{
	internal BasePocketDimensionGeneration([NotNull] object inst)
	{
		if (!CoreTypes.BasePocketDimensionGenerationType.IsInstanceOfType(inst))
		{
			throw new ArgumentException(
				$"Object for BasePocketDimensionGeneration must have parent type {CoreTypes.BasePocketDimensionGenerationType.AssemblyQualifiedName}, but had type {inst.GetType().AssemblyQualifiedName}",
				nameof(inst)
			);
		}

		Inst = inst;
	}

	[NotNull] internal readonly object Inst;
}

public class TemplatePocketDimensionGeneration : BasePocketDimensionGeneration
{
	public TemplatePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		string template,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null
	) : base(
		Activator.CreateInstance(
			CoreTypes.TemplatePocketDimensionGenerationType,
			cyclesLifetime,
			zoneType,
			template,
			requiredSkillId,
			prefabIds
		)
	)
	{
	}
}

public class NoisePocketDimensionGeneration : BasePocketDimensionGeneration
{
	public NoisePocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		[NotNull] List<SimHashes> hashes,
		float xFrequency,
		float yFrequency,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	) : base(
		Activator.CreateInstance(
			CoreTypes.NoisePocketDimensionGenerationType,
			cyclesLifetime,
			zoneType,
			hashes,
			xFrequency,
			yFrequency,
			requiredSkillId,
			prefabIds,
			canSpawnSubDimensions
		)
	)
	{
	}
}

public class CustomPocketDimensionGeneration : BasePocketDimensionGeneration
{
	/// <summary>
	/// Creates an instance of a CustomPocketDimensionGeneration to configure how a pocket dimension can spawn.
	/// Calls a passed function that is expected to place things in the newly created world.
	/// </summary>
	/// <remarks>May only be used if the Twitch mod is active.</remarks>
	/// <param name="cyclesLifetime"></param>
	/// <param name="zoneType"></param>
	/// <param name="generateTilesAction"></param>
	/// <param name="requiredSkillId"></param>
	/// <param name="prefabIds"></param>
	/// <param name="canSpawnSubDimensions"></param>
	public CustomPocketDimensionGeneration(
		float cyclesLifetime,
		SubWorld.ZoneType zoneType,
		Action<WorldContainer> generateTilesAction,
		[CanBeNull] string requiredSkillId = null,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	) : base(
		Activator.CreateInstance(
			CoreTypes.CustomPocketDimensionGenerationType,
			cyclesLifetime,
			zoneType,
			generateTilesAction,
			requiredSkillId,
			prefabIds,
			canSpawnSubDimensions
		)
	)
	{
	}
}
