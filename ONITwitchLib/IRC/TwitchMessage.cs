using System;
using ONITwitchLib.Attributes;

namespace ONITwitchLib.IRC;

#pragma warning disable CS1591
[NotPublicAPI]
public class TwitchMessage(TwitchUserInfo userInfo, string message) : EventArgs
{
	public TwitchUserInfo UserInfo { get; } = userInfo;
	public string Message { get; } = message;
}
