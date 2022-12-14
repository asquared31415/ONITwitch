using System;
using HarmonyLib;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

public class CommandBase
{
	private static readonly Func<object, object> GetRunActionDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CommandExtType, "GetRunAction"),
		null,
		CoreTypes.CommandType,
		typeof(Action<object>)
	);

	private static readonly Func<object, object> GetConditionFuncDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CommandExtType, "GetConditionFunc"),
		null,
		CoreTypes.CommandType,
		typeof(Func<object, bool>)
	);

	private readonly object commandInst;

	public CommandBase(Type commandType, params object[] args)
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

	public CommandBase(object inst)
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

	public Action<object> GetRunAction()
	{
		return (Action<object>) GetRunActionDelegate(commandInst);
	}

	public Func<object, bool> GetConditionFunc()
	{
		return (Func<object, bool>) GetConditionFuncDelegate(commandInst);
	}
}
