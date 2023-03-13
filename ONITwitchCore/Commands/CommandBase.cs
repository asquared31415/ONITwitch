using System;
using JetBrains.Annotations;

namespace ONITwitchCore.Commands;

/// <summary>
/// The base class for all events in the core mod.
/// </summary>
[PublicAPI]
public abstract class CommandBase
{
	/// <summary>
	/// Determines whether a command should be enabled. 
	/// </summary>
	/// <param name="data">The data to test.</param>
	/// <returns><see langword="true"/> if the event should be enabled, otherwise <see langword="false"/>.</returns>
	[PublicAPI]
	public virtual bool Condition(object data)
	{
		return true;
	}

	/// <summary>
	/// The code to run when the event is triggered.
	/// </summary>
	/// <param name="data">The data for the event.</param>
	[PublicAPI]
	public abstract void Run(object data);
}

/// <summary>
/// Extensions for CommandBase, primarily to be used with reflection interface
/// </summary>
[PublicAPI]
public static class CommandBaseExt
{
	/// <summary>
	/// Gets the action that this command will run to determine if it may run.
	/// </summary>
	/// /// <param name="command">The command to get the condition for.</param>
	/// <returns>An <see cref="Action{T}"/> that will call the command's condition.</returns>
	[PublicAPI("Used via merge lib reflection")]
	[Obsolete("Use command.Condition to get the method instead")]
	public static Func<object, bool> GetConditionFunc(this CommandBase command)
	{
		return command.Condition;
	}
	
	/// <summary>
	/// Gets the action that this command will run.
	/// </summary>
	/// <param name="command">The command to get the action for.</param>
	/// <returns>An <see cref="Action{T}"/> that will call the command.</returns>
	[PublicAPI("Used via merge lib reflection")]
	[Obsolete("Use command.Run to get the method instead")]
	public static Action<object> GetRunAction(this CommandBase command)
	{
		return command.Run;
	}

}