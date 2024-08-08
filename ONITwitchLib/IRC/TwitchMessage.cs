using System;
using ONITwitchLib.Attributes;

namespace ONITwitchLib.IRC;

#pragma warning disable CS1591
[NotPublicAPI]
public class TwitchMessage : EventArgs
{
	public TwitchMessage(TwitchUserInfo userInfo, string message)
	{
		UserInfo = userInfo;
		Message = message;
	}

	public TwitchUserInfo UserInfo { get; }
	public string Message { get; }
}
