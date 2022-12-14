using System;
using HarmonyLib;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

public static class PocketDimensionGenerator
{
	private static readonly Action<object> AddGenerationDelegate =
		DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(CoreTypes.PocketDimensionGeneratorType, "AddGenerationConfig"),
			null,
			CoreTypes.BasePocketDimensionGenerationType
		);

	/// <summary>
	/// Adds the specified generation config to the pool for pocket dimensions
	/// </summary>
	/// <remarks>May only be used if the Twitch mod is active.</remarks>
	/// <param name="config"></param>
	public static void AddGenerationConfig(BasePocketDimensionGeneration config)
	{
		AddGenerationDelegate(config.Inst);
	}
}
