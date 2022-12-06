using System;
using JetBrains.Annotations;

namespace ONITwitchCore.Commands;

public abstract class CommandBase
{
	public virtual bool Condition(object data)
	{
		return true;
	}

	public abstract void Run(object data);
}

/// <summary>
/// Extensions for CommandBase, primarily to be used with reflection interface
/// </summary>
public static class CommandBaseExt
{
	[UsedImplicitly] // used by reflection in the lib
	public static Action<object> GetRunAction(this CommandBase command)
	{
		return command.Run;
	}

	[UsedImplicitly] // used by reflection in the lib
	public static Func<object, bool> GetConditionFunc(this CommandBase command)
	{
		return command.Condition;
	}
}