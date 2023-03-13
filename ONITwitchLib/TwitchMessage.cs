using System;

namespace ONITwitchLib;

// TODO: is this public API (maybe not?)
#pragma warning disable CS1591
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
