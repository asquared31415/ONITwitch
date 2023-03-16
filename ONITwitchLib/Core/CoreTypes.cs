using System;
using JetBrains.Annotations;

namespace ONITwitchLib.Core;

/// <summary>
/// Contains <see cref="Type"/>s of many types in the main Twitch mod.
/// </summary>
[PublicAPI]
public static class CoreTypes
{
	// TODO: need to update all these docs

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventManagerType => (eventManagerType ??= Type.GetType(EventManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventInfoType => (eventInfoType ??= Type.GetType(EventInfoTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type DataManagerType => (dataManagerType ??= Type.GetType(DataManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type TwitchDeckManagerType => (twitchDeckManagerType ??= Type.GetType(TwitchDeckManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventGroupType => (eventGroupType ??= Type.GetType(EventGroupTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type DangerType => (dangerType ??= Type.GetType(DangerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CommandType => (commandType ??= Type.GetType(CommandBaseTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CommandExtType => (commandExtType ??= Type.GetType(CommandBaseExtTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type PocketDimensionGeneratorType =>
		(pocketDimensionGeneratorType ??= Type.GetType(PocketDimensionGeneratorTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type BasePocketDimensionGenerationType =>
		(basePocketDimensionGenerationType ??= Type.GetType(BasePocketDimensionGenerationTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type TemplatePocketDimensionGenerationType => (templatePocketDimensionGenerationType ??=
		Type.GetType(TemplatePocketDimensionGenerationTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type NoisePocketDimensionGenerationType =>
		(noisePocketDimensionGenerationType ??= Type.GetType(NoisePocketDimensionGenerationTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CustomPocketDimensionGenerationType => (customPocketDimensionGenerationType ??=
		Type.GetType(CustomPocketDimensionGenerationTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type ToastManagerType => (toastManagerType ??= Type.GetType(ToastManagerTypeName))!;

#region type_internals

	// assembly qualified with the ONITwitch assembly, despite the lib namespace
	private const string DangerTypeName = "ONITwitchLib.Danger, ONITwitch";
	private static Type dangerType;

	private const string CommandBaseTypeName = "ONITwitchCore.Commands.CommandBase, ONITwitch";
	private static Type commandType;

	private const string CommandBaseExtTypeName = "ONITwitchCore.Commands.CommandBaseExt, ONITwitch";
	private static Type commandExtType;

	private const string PocketDimensionGeneratorTypeName =
		"ONITwitchCore.Content.PocketDimensionGenerator, ONITwitch";

	private static Type pocketDimensionGeneratorType;

	// Base pocket dimension generation config
	private const string BasePocketDimensionGenerationTypeName =
		"ONITwitchCore.Content.BasePocketDimensionGeneration, ONITwitch";

	private static Type basePocketDimensionGenerationType;

	// Template-based pocket dimension generation config
	private const string TemplatePocketDimensionGenerationTypeName =
		"ONITwitchCore.Content.TemplatePocketDimensionGeneration, ONITwitch";

	private static Type templatePocketDimensionGenerationType;

	// Noise-based pocket dimension generation config
	private const string NoisePocketDimensionGenerationTypeName =
		"ONITwitchCore.Content.NoisePocketDimensionGeneration, ONITwitch";

	private static Type noisePocketDimensionGenerationType;

	// Custom function pocket dimension generation config
	private const string CustomPocketDimensionGenerationTypeName =
		"ONITwitchCore.Content.CustomPocketDimensionGeneration, ONITwitch";

	private static Type customPocketDimensionGenerationType;

	private const string ToastManagerTypeName = "ONITwitchCore.Toasts.ToastManager, ONITwitch";
	private static Type toastManagerType;

	private const string EventManagerTypeName = "EventLib.EventManager, ONITwitch";
	private static Type eventManagerType;

	private const string EventInfoTypeName = "EventLib.EventInfo, ONITwitch";
	private static Type eventInfoType;

	private const string DataManagerTypeName = "EventLib.DataManager, ONITwitch";
	private static Type dataManagerType;

	private const string TwitchDeckManagerTypeName = "ONITwitchCore.TwitchDeckManager, ONITwitch";
	private static Type twitchDeckManagerType;

	private const string EventGroupTypeName = "EventLib.EventGroup, ONITwitch";
	private static Type eventGroupType;

#endregion
}
