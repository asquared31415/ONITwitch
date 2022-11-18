using System;

namespace ONITwitchLib;

public class TwitchMessage : EventArgs
{
	public TwitchUserInfo UserInfo { get; }
	public string Message { get; }

	public TwitchMessage(TwitchUserInfo userInfo, string message)
	{
		UserInfo = userInfo;
		Message = message;
	}
}
