using System;
using HarmonyLib;

namespace ONITwitchLib.Core;

public class CommandBase
{
	private static readonly Func<object, object> GetRunActionDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CoreCommandExtType, "GetRunAction"),
		null,
		CoreTypes.CoreCommandType,
		typeof(Action<object>)
	);

	private static readonly Func<object, object> GetConditionFuncDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(CoreTypes.CoreCommandExtType, "GetConditionFunc"),
		null,
		CoreTypes.CoreCommandType,
		typeof(Func<object, bool>)
	);

	private readonly object commandInst;

	public CommandBase(object inst)
	{
		commandInst = inst;

		var instType = commandInst.GetType();
		if (!CoreTypes.CoreCommandType.IsAssignableFrom(instType))
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
