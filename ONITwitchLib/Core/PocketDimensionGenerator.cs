using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

/// <summary>
/// Provides methods for adding new pocket dimensions to the generation pool and to generate pocket dimensions.
/// TODO: implement this
/// </summary>
[PublicAPI]
public static class PocketDimensionGenerator
{
/*
	/// <summary>
	/// Adds the specified generation config to the pool for pocket dimensions.
	/// </summary>
	/// <param name="config">The generation to add.</param>
	/// <seealso cref="BasePocketDimensionGeneration"/>
	/// <seealso cref="TemplatePocketDimensionGeneration"/>
	/// <seealso cref="NoisePocketDimensionGeneration"/>
	/// <seealso cref="CustomPocketDimensionGeneration"/>
	[PublicAPI]
	public static void AddGenerationConfig(BasePocketDimensionGeneration config)
	{
		AddGenerationDelegate(config.Inst);
	}

	// TODO: Add GenerateDimension

	private static readonly Action<object> AddGenerationDelegate =
		DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(CoreTypes.PocketDimensionGeneratorType, "AddGenerationConfig"),
			null,
			CoreTypes.BasePocketDimensionGenerationType
		);
	*/
}
