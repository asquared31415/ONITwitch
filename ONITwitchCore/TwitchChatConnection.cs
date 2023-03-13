using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ONITwitchCore.Config;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitchCore;

internal class TwitchChatConnection
{
	private enum CapStatus
	{
		Requested,
		Enabled,
		NotAcknowledged,
	}

	private static readonly Uri TwitchChatUri = new("wss://irc-ws.chat.twitch.tv:443");

	private readonly ClientWebSocket socket = new();

	private CancellationTokenSource socketCancellationTokenSource;

	private readonly Dictionary<string, CapStatus> capStatuses = new();

	private readonly ConcurrentQueue<IrcMessage> messageQueue = new();

	public bool IsAuthenticated { get; private set; }

	public delegate void TwitchMessageHandler(TwitchMessage message);

	public event TwitchMessageHandler OnTwitchMessage;

	public void Start()
	{
		Task.Run(
			async () =>
			{
				var credentials = CredentialsConfig.Instance.Credentials;
				if (!credentials.IsValid())
				{
					// TODO: handle this whole validity checking and erroring better
					Log.Warn("Credentials are not valid");
					return;
				}

				await Connect();
				socketCancellationTokenSource = new CancellationTokenSource();
				StartReader(socketCancellationTokenSource.Token);
				StartWriter(socketCancellationTokenSource.Token);

				RequestCaps();
				Authenticate(CredentialsConfig.Instance.Credentials);
			}
		);
	}

	private void StartReader(CancellationToken cancellationToken)
	{
		Task.Run(
			async () =>
			{
				while (true)
				{
					cancellationToken.ThrowIfCancellationRequested();

					var result = await GetMessage();
					if (result == null)
					{
						Log.Debug("Unable to read message");
					}
					else
					{
						var messages = result.Split('\n');
						foreach (var message in messages)
						{
							var trimmed = message.TrimEnd('\r', '\n');
							if (string.IsNullOrWhiteSpace(trimmed))
							{
								continue;
							}

							var command = IrcParser.ParseMessage(trimmed);
							if (command.HasValue)
							{
								ProcessMessage(command.Value);
							}
							else
							{
								Log.Warn($"Unable to parse IRC message: {trimmed}");
							}
						}
					}

					Thread.Sleep(50);
				}
				// ReSharper disable once FunctionNeverReturns : intentional loop
			},
			cancellationToken
		);
	}

	private void StartWriter(CancellationToken cancellationToken)
	{
		Task.Run(
			async () =>
			{
				while (true)
				{
					while (messageQueue.TryDequeue(out var message))
					{
						var textMessage = message.GetIrcString();
						/*
						if (!message.Command.IsNumeric && (message.Command.Command.Value == IrcCommandType.PASS))
						{
							Debug.Log("Sending IRC message: PASS <REDACTED>");
						}
						else
						{
							Debug.Log($"Sending IRC message: {textMessage}");
						}
						*/

						await SendRawTextMessage(textMessage);
					}

					await Task.Delay(50, cancellationToken);
				}
				// ReSharper disable once FunctionNeverReturns : intentional loop
			},
			cancellationToken
		);
	}

	public void SendIrcMessage(IrcMessage message)
	{
		if (message.Command.IsKnownCommand())
		{
			messageQueue.Enqueue(message);
		}
		else
		{
			Log.Warn($"Cannot send unknown IRC command {message.Command}");
		}
	}

	public void SendTextMessage([NotNull] string room, [NotNull] string message)
	{
		if (!room.StartsWith("#"))
		{
			room = $"#{room}";
		}

		room = room.ToLowerInvariant();

		var privMsg = new IrcMessage(IrcCommandType.PRIVMSG, new List<string> { room, message });
		SendIrcMessage(privMsg);
	}

	public void JoinRoom(string room)
	{
		if (!room.StartsWith("#"))
		{
			room = $"#{room}";
		}

		room = room.ToLowerInvariant();

		var joinMessage = new IrcMessage(IrcCommandType.JOIN, new List<string> { room });
		messageQueue.Enqueue(joinMessage);
	}

	private void ProcessMessage(IrcMessage message)
	{
		if (message.Command.IsNumeric)
		{
			// ReSharper disable once PossibleInvalidOperationException
			// checked above
			var commandId = message.Command.NumericId.Value;
			switch (commandId)
			{
				// auth flavor messages
				case 1:
				case 2:
				case 3:
				case 4:
				{
					break;
				}
				// RPL_NAMREPLY
				case 353:
				{
					break;
				}
				// RPL_ENDOFNAMES
				case 366:
				{
					break;
				}
				// RPL_MOTDSTART
				case 375:
				{
					break;
				}
				// RPL_MOTD
				case 372:
				{
					if (message.Args.LastOrDefault() is string motd)
					{
						Log.Debug($"Twitch IRC MOTD: {motd}");
					}

					break;
				}
				// RPL_ENDOFMOTD
				case 376:
				{
					break;
				}
			}
		}
		else
		{
			// ReSharper disable once PossibleInvalidOperationException
			// checked by not being numeric
			var command = message.Command.Command.Value;
			switch (command)
			{
				case IrcCommandType.JOIN:
					break;
				case IrcCommandType.NICK:
					break;
				case IrcCommandType.NOTICE:
					break;
				case IrcCommandType.PART:
					break;
				case IrcCommandType.PASS:
					break;
				case IrcCommandType.PING:
				{
					var pingArg = message.Args.LastOrDefault();
					if (pingArg == null)
					{
						Log.Debug("Received null ping arg");
					}
					else
					{
						var pongMsg = new IrcMessage(new IrcCommand(IrcCommandType.PONG), new List<string> { pingArg });
						SendIrcMessage(pongMsg);
					}

					break;
				}
				case IrcCommandType.PONG:
				{
					Log.Debug("unexpected PONG received");
					break;
				}
				case IrcCommandType.PRIVMSG:
				{
					HandlePrivMsg(message);
					break;
				}
				case IrcCommandType.CAP:
				{
					if (message.Args.Count < 2)
					{
						Log.Debug("Did not receive CAP subcommand");
						break;
					}

					var subcommand = message.Args[1];

					switch (subcommand)
					{
						case "ACK":
						{
							if (message.Args.Count >= 3)
							{
								var acknowledgedCaps = message.Args[2].Split(' ');
								foreach (var acknowledgedCap in acknowledgedCaps)
								{
									capStatuses[acknowledgedCap] = CapStatus.Enabled;
								}
							}

							break;
						}
						case "NAK":
						{
							if (message.Args.Count >= 3)
							{
								var nakCaps = message.Args[2].Split(' ');
								foreach (var cap in nakCaps)
								{
									capStatuses[cap] = CapStatus.NotAcknowledged;
								}
							}

							break;
						}
					}

					break;
				}
				case IrcCommandType.CLEARCHAT:
					break;
				case IrcCommandType.CLEARMSG:
					break;
				case IrcCommandType.GLOBALUSERSTATE:
				{
					IsAuthenticated = true;
					break;
				}
				case IrcCommandType.HOSTTARGET:
					break;
				case IrcCommandType.RECONNECT:
					break;
				case IrcCommandType.ROOMSTATE:
					break;
				case IrcCommandType.USERNOTICE:
					break;
				case IrcCommandType.USERSTATE:
					break;
				case IrcCommandType.WHISPER:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void HandlePrivMsg(IrcMessage message)
	{
		if (message.Args.Count < 2)
		{
			Log.Debug("PRIVMSG did not include all args");
			return;
		}

		var msg = message.Args[1];
		var tags = message.Tags;

		var userId = tags["user-id"].Value;

		// use the display-name tag if it exists, otherwise use the message nick
		var displayName = message.Args[0].TrimStart('#');
		if (tags.TryGetValue("display-name", out var displayTag))
		{
			if (!displayTag.Value.IsNullOrWhiteSpace())
			{
				displayName = displayTag.Value;
			}
		}

		Color32? color = null;
		if (tags.TryGetValue("color", out var colorTag) && !colorTag.Value.IsNullOrWhiteSpace())
		{
			var colorStr = colorTag.Value!;
			if (ColorUtil.TryParseHexString(colorStr, out var parsed))
			{
				color = parsed;
			}
		}

		var isModerator = false;
		if (tags.TryGetValue("mod", out var isMod))
		{
			isModerator = isMod.Value == "1";
		}

		var isSubscriber = false;
		if (tags.TryGetValue("subscriber", out var isSub))
		{
			isSubscriber = isSub.Value == "1";
		}

		// VIP is weird, the *presence* of the key is the true/false value
		var isVip = tags.ContainsKey("vip");

		var userInfo = new TwitchUserInfo(userId, displayName, color, isModerator, isSubscriber, isVip);
		var twitchMessage = new TwitchMessage(userInfo, msg);
		MainThreadScheduler.Schedule(
			() =>
			{
				// make value copy so that it doesn't race with the null check	
				var onMsg = OnTwitchMessage;
				onMsg?.Invoke(twitchMessage);
			}
		);
	}

	private async Task Connect()
	{
		// maximum number of attempts to make to connect to Twitch, backing off exponentially each time
		const int maxConnectAttempts = 10;
		for (var attempt = 0; attempt < maxConnectAttempts; attempt++)
		{
			try
			{
				await socket.ConnectAsync(TwitchChatUri, CancellationToken.None);
				break;
			}
			catch (AggregateException ae)
			{
				foreach (var ie in ae.InnerExceptions)
				{
					switch (ie)
					{
						case WebSocketException we:
						{
							Log.Warn(we);
							break;
						}
						case Exception:
						{
							// the interleaved warns and debugs are intentional so that a release build doesn't
							// have extra crap in it
							Log.Warn("An unexpected exception occurred when connecting");
							Log.Debug(ie.GetType());
							Log.Warn(ie.Message);
							Log.Debug(ie.StackTrace);
							return;
						}
					}
				}
			}

			await Task.Delay(Mathf.FloorToInt(500 * Mathf.Pow(2, attempt)));
		}
	}

	private void RequestCaps()
	{
		RequestCap("twitch.tv/commands");
		RequestCap("twitch.tv/tags");
	}

	// returns whether the capability was requested
	private void RequestCap([NotNull] string capName)
	{
		if (capStatuses.TryGetValue(capName, out var capStatus) && (capStatus == CapStatus.Enabled))
		{
			// already requested, so exit early
			return;
		}

		capStatuses[capName] = CapStatus.Requested;

		var capMsg = new IrcMessage(IrcCommandType.CAP, new List<string> { "REQ", capName });
		SendIrcMessage(capMsg);
	}

	private bool CheckCapsEnabled()
	{
		foreach (var (_, status) in capStatuses)
		{
			if (status != CapStatus.Enabled)
			{
				return false;
			}
		}

		return true;
	}

	private void Authenticate(Credentials credentials)
	{
		var passMsg = new IrcMessage(IrcCommandType.PASS, new List<string> { credentials.Oauth });
		SendIrcMessage(passMsg);

		var nickMsg = new IrcMessage(IrcCommandType.NICK, new List<string> { credentials.Nick.ToLowerInvariant() });
		SendIrcMessage(nickMsg);
	}

	private async Task SendRawTextMessage([NotNull] string message)
	{
		if (!message.EndsWith("\r\n"))
		{
			// remove all counts of either \r or \n and then re-add to normalize it
			message = message.TrimEnd('\r', '\n') + "\r\n";
		}

		var data = Encoding.UTF8.GetBytes(message);
		try
		{
			await socket.SendAsync(
				new ArraySegment<byte>(data),
				WebSocketMessageType.Text,
				true,
				CancellationToken.None
			);
		}
		catch (AggregateException ae)
		{
			Log.Warn("Unable to send IRC message");
			foreach (var ie in ae.InnerExceptions)
			{
				Log.Warn(ie.Message);
				Log.Debug(ie);
			}
		}
	}

	private async Task<string> GetMessage()
	{
		var buffer = new ArraySegment<byte>(new byte[8192]);

		using var s = new MemoryStream();
		try
		{
			WebSocketReceiveResult result;
			do
			{
				result = await socket.ReceiveAsync(buffer, CancellationToken.None);
				s.Write(buffer.Array!, buffer.Offset, result.Count);
			} while (result.EndOfMessage != true);
		}
		catch (AggregateException ae)
		{
			Log.Debug(socket.State);
			Log.Debug(socket.CloseStatus);
			Log.Debug(socket.CloseStatusDescription);
			foreach (var ie in ae.InnerExceptions)
			{
				Log.Debug(ie);
			}

			return null;
		}

		var bytes = s.ToArray();
		return Encoding.UTF8.GetString(bytes);
	}
}
