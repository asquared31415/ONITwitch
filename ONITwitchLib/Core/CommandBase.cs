using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

/// <summary>
///     Methods for accessing the core Twitch mod's commands.
/// </summary>
[PublicAPI]
public class CommandBase
{
	private static readonly Func<object, object> GetRunActionDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CommandExtType, "GetRunAction", new[] { CoreTypes.CommandType }),
		null,
		CoreTypes.CommandType,
		typeof(Action<object>)
	);

	private static readonly Func<object, object> GetConditionFuncDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CommandExtType, "GetConditionFunc", new[] { CoreTypes.CommandType }),
		null,
		CoreTypes.CommandType,
		typeof(Func<object, bool>)
	);

	[NotNull] private readonly object commandInst;

	/// <summary>
	///     Instantiates a <see cref="CommandBase" /> using a type and its arguments.
	/// </summary>
	/// <param name="commandType">A type deriving from the core mod's <c>CommandBase</c> type.</param>
	/// <param name="args">The arguments for that type's constructor.</param>
	[PublicAPI]
	public CommandBase([NotNull] Type commandType, params object[] args)
	{
		if (!CoreTypes.CommandType.IsAssignableFrom(commandType))
		{
			throw new ArgumentException(
				$"[Twitch Integration] lib attempted to create a CommandBase with invalid type {commandType}",
				nameof(commandType)
			);
		}

		commandInst = Activator.CreateInstance(commandType, args);
	}

	/// <summary>
	///     Instantiates a <see cref="CommandBase" /> using an instance of the core mod's <c>CommandBase</c>.
	/// </summary>
	/// <param name="inst">The instance to use.</param>
	[PublicAPI]
	public CommandBase([NotNull] object inst)
	{
		commandInst = inst;

		var instType = commandInst.GetType();
		if (!CoreTypes.CommandType.IsAssignableFrom(instType))
		{
			throw new ArgumentException(
				$"[Twitch Integration] lib attempted to create a CommandBase with invalid type {instType}",
				nameof(inst)
			);
		}
	}

	/// <summary>
	///     Gets the action that this command will run.
	/// </summary>
	/// <returns>An <see cref="Action{T}" /> that will call the command.</returns>
	[PublicAPI]
	public Action<object> GetRunAction()
	{
		return (Action<object>) GetRunActionDelegate(commandInst);
	}

	/// <summary>
	///     Gets the action that this command will run to determine if it may run.
	/// </summary>
	/// <returns>An <see cref="Action{T}" /> that will call the command's condition.</returns>
	[PublicAPI]
	public Func<object, bool> GetConditionFunc()
	{
		return (Func<object, bool>) GetConditionFuncDelegate(commandInst);
	}
}
