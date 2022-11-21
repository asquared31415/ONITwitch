namespace ONITwitchCore.Commands;

public abstract class CommandBase
{
	public virtual bool Condition(object data)
	{
		return true;
	}

	public abstract void Run(object data);
}
