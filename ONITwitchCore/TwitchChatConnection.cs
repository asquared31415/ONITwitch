using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ONITwitchLib;

namespace ONITwitchCore;

public class TwitchChatConnection
{
	public enum CapStatus
	{
		Requested,
		Enabled,
		NotAcknowledged,
	}

	public static readonly Uri TwitchChatUri = new("wss://irc-ws.chat.twitch.tv:443");

	private readonly ClientWebSocket socket = new();

	private CancellationTokenSource readerCancellationToken;

	private readonly Dictionary<string, CapStatus> capStatuses = new();
	private bool isAuthenticated;

	public TwitchChatConnection()
	{
	}

	public void Start()
	{
		// all processing should be done on another thread
		Task.Run(
			() =>
			{
				var connectStatus = Connect();
				if (connectStatus.State != ConnectionResult.ResultState.Open)
				{
					Debug.LogWarning($"[Twitch Integration] socket in unsuccessful status {connectStatus.State}");
					return;
				}

				readerCancellationToken = new CancellationTokenSource();
				var readerTask = StartReader(readerCancellationToken.Token);

				RequestCaps();

				Authenticate();

				var checkCapsCount = 0;
				while (!CheckCapsEnabled() && (checkCapsCount < 40))
				{
					checkCapsCount += 1;
					Thread.Sleep(50);
				}

				if (checkCapsCount >= 40)
				{
					var sb = new StringBuilder();
					foreach (var (key, status) in capStatuses)
					{
						if (status != CapStatus.Enabled)
						{
							sb.Append(key);
							sb.Append(" ");
						}
					}

					Debug.LogWarning(
						$"[Twitch Integration] the following capabilities were not able to be requested:\n\t{sb}"
					);
					//readerCancellationToken.Cancel();
				}

				readerTask.Wait();
			}
		);
	}

	public void SendMessage(IrcMessage message)
	{
		if (message.Command.IsKnownCommand())
		{
			var ircMessage = message.GetIrcString();
			if (!message.Command.IsNumeric && (message.Command.Command.Value == IrcCommandType.PASS))
			{
				Debug.Log("Sending IRC message: PASS <REDACTED>");
			}
			else
			{
				Debug.Log($"Sending IRC message: {ircMessage}");
			}

			SendMessage(ircMessage);
		}
		else
		{
			Debug.LogWarning($"[Twitch Integration] Cannot send unknown command {message.Command}");
		}
	}

	private Task StartReader(CancellationToken cancellationToken)
	{
		return Task.Run(
			() =>
			{
				while (true)
				{
					cancellationToken.ThrowIfCancellationRequested();

					var result = GetMessage();
					if (result == null)
					{
						Debug.LogWarning("[Twitch Integration] Unable to read message");
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
								Debug.LogWarning($"[Twitch Integration] Unable to parse message: {trimmed}");
							}
						}
					}

					Thread.Sleep(50);
				}
			},
			cancellationToken
		);
	}

	private void ProcessMessage(IrcMessage message)
	{
		Debug.Log($"Processing command {message}");
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
						Debug.Log($"[Twitch Integration] MOTD: {motd}");
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
						Debug.LogWarning("[Twitch Integration] Received null ping arg");
					}
					else
					{
						Debug.Log($"Got ping {pingArg}");
						var pongMsg = new IrcMessage(new IrcCommand(IrcCommandType.PONG), new List<string> { pingArg });
						SendMessage(pongMsg);
					}

					break;
				}
				case IrcCommandType.PONG:
				{
					Debug.LogWarning("[Twitch Integration] unexpected PONG received");
					break;
				}
				case IrcCommandType.PRIVMSG:
					break;
				case IrcCommandType.CAP:
				{
					if (message.Args.Count < 2)
					{
						Debug.LogWarning("[Twitch Integration] Did not receive CAP subcommand");
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
					isAuthenticated = true;
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

	private ConnectionResult Connect()
	{
		try
		{
			socket.ConnectAsync(TwitchChatUri, CancellationToken.None).Wait();
		}
		catch (AggregateException ae)
		{
			foreach (var ie in ae.InnerExceptions)
			{
				switch (ie)
				{
					case WebSocketException we:
					{
						return new ConnectionResult { State = ConnectionResult.ResultState.Error, ErrorException = we };
					}
					case Exception e:
					{
						Debug.LogWarning("An unexpected exception occurred when connecting");
						Debug.Log(e.GetType());
						Debug.Log(e.Message);
						Debug.Log(e.StackTrace);
						return new ConnectionResult { State = ConnectionResult.ResultState.UnknownException };
					}
				}
			}
		}

		return socket.State == WebSocketState.Open
			? new ConnectionResult { State = ConnectionResult.ResultState.Open }
			: new ConnectionResult { State = ConnectionResult.ResultState.Closed, CloseState = socket.CloseStatus };
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
		SendMessage(capMsg);
	}

	private bool CheckCapsEnabled()
	{
		foreach (var (key, status) in capStatuses)
		{
			if (status != CapStatus.Enabled)
			{
				return false;
			}
		}

		return true;
	}

	private void Authenticate()
	{
		// TODO: FIX THESE
		const string oauth = "";
		const string nick = "";

		var passMsg = new IrcMessage(IrcCommandType.PASS, new List<string> { oauth });
		SendMessage(passMsg);

		var nickMsg = new IrcMessage(IrcCommandType.NICK, new List<string> { nick });
		SendMessage(nickMsg);
	}

	// returns true if the message was successfully sent
	private void SendMessage([NotNull] string message)
	{
		if (!message.EndsWith("\r\n"))
		{
			// remove all counts of either \r or \n and then readd to normalize it
			message = message.TrimEnd('\r', 'n') + "\r\n";
		}

		var data = Encoding.UTF8.GetBytes(message);
		try
		{
			socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None)
				.Wait();
		}
		catch (AggregateException ae)
		{
			foreach (var ie in ae.InnerExceptions)
			{
				Debug.LogWarning(ie);
			}
		}
	}

	[CanBeNull]
	private string GetMessage()
	{
		var buffer = new ArraySegment<byte>(new byte[8192]);

		using var s = new MemoryStream();
		try
		{
			WebSocketReceiveResult result;
			do
			{
				result = socket.ReceiveAsync(buffer, CancellationToken.None).Result;
				s.Write(buffer.Array!, buffer.Offset, result.Count);
			} while (result.EndOfMessage != true);
		}
		catch (AggregateException ae)
		{
			foreach (var ie in ae.InnerExceptions)
			{
				Debug.LogWarning(ie);
			}

			return null;
		}

		var bytes = s.ToArray();
		return Encoding.UTF8.GetString(bytes);
	}
}

public struct ConnectionResult
{
	public ResultState State;

	// only present if the state is Closed
	[CanBeNull] public WebSocketCloseStatus? CloseState;

	// only present if the state is Error
	[CanBeNull] public WebSocketException ErrorException;

	public enum ResultState
	{
		NotConnected,
		Open,
		Closed,
		Error,
		UnknownException,
	}

	public override string ToString()
	{
		return State switch
		{
			ResultState.NotConnected => "Not yet connected",
			ResultState.Open => "Open",
			ResultState.Closed => $"Closed ({CloseState})",
			ResultState.Error => $"Error ({ErrorException})",
			ResultState.UnknownException => "Unknown Exception",
			_ => throw new ArgumentOutOfRangeException(),
		};
	}
}

public struct ReceiveMessageResult
{
	public bool IsSuccess { get; internal set; }
	[CanBeNull] public string Message { get; internal set; }
}
