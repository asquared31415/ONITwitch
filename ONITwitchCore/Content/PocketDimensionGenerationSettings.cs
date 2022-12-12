using System.Collections.Generic;
using JetBrains.Annotations;
using ProcGen;

namespace ONITwitchCore.Content;

public record struct PocketDimensionGenerationSettings(
	float CyclesLifetime,
	[CanBeNull] string RequiredSkillId,
	SubWorld.ZoneType ZoneType,
	[NotNull] List<SimHashes> Hashes,
	float XFrequency,
	float YFrequency
);
