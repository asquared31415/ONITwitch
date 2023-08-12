using System;
using JetBrains.Annotations;

namespace ONITwitchLib.Core;

/// <summary>
///     Contains <see cref="Type" />s of many types in the main Twitch Integration mod.
/// </summary>
/// <remarks>
///     These types are only present if the Twitch Integration mod is active.
/// </remarks>
[PublicAPI]
public static class CoreTypes
{
	/// <summary>
	///     The type of the <see cref="EventManager" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type EventManagerType => eventManagerType ??= Type.GetType(EventManagerTypeName)!;

	/// <summary>
	///     The type of <see cref="EventInfo" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type EventInfoType => eventInfoType ??= Type.GetType(EventInfoTypeName)!;

	/// <summary>
	///     The type of the <see cref="DataManager" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type DataManagerType => dataManagerType ??= Type.GetType(DataManagerTypeName)!;

	/// <summary>
	///     The type of the <see cref="TwitchDeckManager" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type TwitchDeckManagerType => twitchDeckManagerType ??= Type.GetType(TwitchDeckManagerTypeName)!;

	/// <summary>
	///     The type of <see cref="EventGroup" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type EventGroupType => eventGroupType ??= Type.GetType(EventGroupTypeName)!;

	/// <summary>
	///     The type of <see cref="Danger" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type DangerType => dangerType ??= Type.GetType(DangerTypeName)!;

	/// <summary>
	///     The type of <see cref="CommandBase" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type CommandType => commandType ??= Type.GetType(CommandBaseTypeName)!;

	/// <summary>
	///     The type of CommandBaseExt from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type CommandExtType => commandExtType ??= Type.GetType(CommandBaseExtTypeName)!;

	/// <summary>
	///     The type of the <see cref="ToastManager" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type ToastManagerType => toastManagerType ??= Type.GetType(ToastManagerTypeName)!;

	/// <summary>
	///     The type of <see cref="TwitchSettings" /> from the core Twitch Integration mod.
	///     Only present if the Twitch Integration mod is active.
	/// </summary>
	[NotNull]
	public static Type TwitchSettingsType => twitchSettingsType ??= Type.GetType(TwitchSettingsTypeName)!;

#region type_internals

	// assembly qualified with the ONITwitch assembly, despite the lib namespace
	private const string DangerTypeName = "ONITwitchLib.Danger, ONITwitch";
	private static Type dangerType;

	private const string CommandBaseTypeName = "ONITwitch.Commands.CommandBase, ONITwitch";
	private static Type commandType;

	private const string CommandBaseExtTypeName = "ONITwitch.Commands.CommandBaseExt, ONITwitch";
	private static Type commandExtType;

#pragma warning disable CS0169
	private const string PocketDimensionGeneratorTypeName =
		"ONITwitch.Content.PocketDimensionGenerator, ONITwitch";

	private static Type pocketDimensionGeneratorType;

	// Base pocket dimension generation config
	private const string BasePocketDimensionGenerationTypeName =
		"ONITwitch.Content.BasePocketDimensionGeneration, ONITwitch";

	private static Type basePocketDimensionGenerationType;

	// Template-based pocket dimension generation config
	private const string TemplatePocketDimensionGenerationTypeName =
		"ONITwitch.Content.TemplatePocketDimensionGeneration, ONITwitch";

	private static Type templatePocketDimensionGenerationType;

	// Noise-based pocket dimension generation config
	private const string NoisePocketDimensionGenerationTypeName =
		"ONITwitch.Content.NoisePocketDimensionGeneration, ONITwitch";

	private static Type noisePocketDimensionGenerationType;

	// Custom function pocket dimension generation config
	private const string CustomPocketDimensionGenerationTypeName =
		"ONITwitch.Content.CustomPocketDimensionGeneration, ONITwitch";

	private static Type customPocketDimensionGenerationType;
#pragma warning restore CS0169

	private const string ToastManagerTypeName = "ONITwitch.Toasts.ToastManager, ONITwitch";
	private static Type toastManagerType;

	private const string EventManagerTypeName = "ONITwitch.EventLib.EventManager, ONITwitch";
	private static Type eventManagerType;

	private const string EventInfoTypeName = "ONITwitch.EventLib.EventInfo, ONITwitch";
	private static Type eventInfoType;

	private const string DataManagerTypeName = "ONITwitch.EventLib.DataManager, ONITwitch";
	private static Type dataManagerType;

	private const string TwitchDeckManagerTypeName = "ONITwitch.TwitchDeckManager, ONITwitch";
	private static Type twitchDeckManagerType;

	private const string EventGroupTypeName = "ONITwitch.EventLib.EventGroup, ONITwitch";
	private static Type eventGroupType;

	private const string TwitchSettingsTypeName = "ONITwitch.Settings.Components.TwitchSettings, ONITwitch";
	private static Type twitchSettingsType;

#endregion
}
