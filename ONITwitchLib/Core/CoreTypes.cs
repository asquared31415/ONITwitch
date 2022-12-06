using System;
using JetBrains.Annotations;

namespace ONITwitchLib.Core;

public static class CoreTypes
{
	// assembly qualified with the ONITwitch assembly, despite the lib namespace
	private const string CoreDangerTypeName = "ONITwitchLib.Danger, ONITwitch";
	private static Type coreDangerType;

	private const string CoreCommandBaseTypeName = "ONITwitchCore.Commands.CommandBase, ONITwitch";
	private static Type coreCommandType;

	private const string CoreCommandBaseExtTypeName = "ONITwitchCore.Commands.CommandBaseExt, ONITwitch";
	private static Type coreCommandExtType;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CoreDangerType => (coreDangerType ??= Type.GetType(CoreDangerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CoreCommandType => (coreCommandType ??= Type.GetType(CoreCommandBaseTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CoreCommandExtType => (coreCommandExtType ??= Type.GetType(CoreCommandBaseExtTypeName))!;
}
