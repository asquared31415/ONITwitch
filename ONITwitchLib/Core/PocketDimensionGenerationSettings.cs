using System.Collections.Generic;
using JetBrains.Annotations;
using ProcGen;

namespace ONITwitchLib.Core;

public class PocketDimensionGenerationSettings
{
	public float CyclesLifetime;
	[CanBeNull] public string RequiredSkillId;
	public SubWorld.ZoneType ZoneType;

	public readonly GenerationKind Kind;

	// noise generation
	public List<SimHashes> Hashes;
	public float XFrequency;
	public float YFrequency;

	// template generation
	public string Template;

	[CanBeNull] public List<string> PrefabIds;

	public bool CanSpawnSubDimensions;

	public PocketDimensionGenerationSettings(
		float cyclesLifetime,
		[CanBeNull] string requiredSkillId,
		SubWorld.ZoneType zoneType,
		List<SimHashes> hashes,
		float xFrequency,
		float yFrequency,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	)
	{
		CyclesLifetime = cyclesLifetime;
		RequiredSkillId = requiredSkillId;
		ZoneType = zoneType;
		Kind = GenerationKind.Noise;
		Hashes = hashes;
		XFrequency = xFrequency;
		YFrequency = yFrequency;
		PrefabIds = prefabIds;
		CanSpawnSubDimensions = canSpawnSubDimensions;
	}

	public PocketDimensionGenerationSettings(
		float cyclesLifetime,
		[CanBeNull] string requiredSkillId,
		SubWorld.ZoneType zoneType,
		string template,
		[CanBeNull] List<string> prefabIds = null,
		bool canSpawnSubDimensions = true
	)
	{
		CyclesLifetime = cyclesLifetime;
		RequiredSkillId = requiredSkillId;
		ZoneType = zoneType;
		Kind = GenerationKind.Template;
		Template = template;
		PrefabIds = prefabIds;
		CanSpawnSubDimensions = canSpawnSubDimensions;
	}

	public enum GenerationKind
	{
		Noise,
		Template
	}
}
