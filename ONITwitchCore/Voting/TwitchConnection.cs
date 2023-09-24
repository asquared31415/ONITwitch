using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ONITwitch.Config;
using ONITwitch.Content.Cmps;
using ONITwitchLib.IRC;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using STRINGS;
using UnityEngine;

namespace ONITwitch.Voting;

internal class TwitchConnection
{
	private static readonly Uri TwitchChatUri = new("wss://irc-ws.chat.twitch.tv:443");

	[NotNull] private readonly Queue<IrcMessage> incomingMessages = new();

	[NotNull] private readonly HashSet<string> joinedChannels = new();

	[NotNull] private readonly ConcurrentQueue<IrcMessage> outgoingMessages = new();

	private long lastPingUtcTicks;

	[NotNull] private ClientWebSocket socket = new();
	public bool IsReady { get; private set; }

	/// <summary>
	///     This event is called when the connection is ready and properly authenticated.
	///     This may not be called on the main thread!
	/// </summary>
	public event System.Action OnReady;

	/// <summary>
	///     This event is called when the connection receives any non-system message.
	///     This may not be called on the main thread!
	/// </summary>
	// ReSharper disable once EventNeverSubscribedTo.Global
	public event Action<IrcMessage> OnMessage;

	/// <summary>
	///     This event is called when a twitch chat message is encountered.
	///     This may not be called on the main thread!
	/// </summary>
	public event Action<TwitchMessage> OnChatMessage;

	// creates a new thread for the twitch connection
	public void Start(Credentials credentials)
	{
		Task.Run(
			async () =>
			{
				// loop up to 5 times if the connection gets dropped/disconnected
				const int maxAttempts = 5;
				var attempts = 0;
				do
				{
					IsReady = false;

					attempts += 1;
					Log.Debug($"Connection attempt {attempts}");

					// big try catch so that things get logged always, instead of silently ignored
					try
					{
						await Connect();
						Log.Info("Connected to Twitch");

						// Request caps and log in
						await Login(credentials);

						IsReady = true;

						// copy to prevent race conditions if event is concurrently modified
						var ready = OnReady;
						ready?.Invoke();

						var cancellation = new CancellationTokenSource();

						// set up a task that monitors the last time that a PING was received from Twitch
						lastPingUtcTicks = System.DateTime.UtcNow.Ticks;
						var timeoutTask = Task.Run(
							async () =>
							{
								while (true)
								{
									cancellation.Token.ThrowIfCancellationRequested();

									var original = Interlocked.Read(ref lastPingUtcTicks);
									var diff = TimeSpan.FromTicks(System.DateTime.UtcNow.Ticks - original);
									const double minuteTimeout = 8;
									if (diff.TotalMinutes > minuteTimeout)
									{
										Log.Warn(
											$"No PING received from Twitch in {minuteTimeout} minutes, assuming the connection to be dead."
										);
										cancellation.Cancel();
										break;
									}

									await Task.Delay(15000, cancellation.Token);
								}
							},
							cancellation.Token
						);


						// Start up reader and writer threads
						// From this point forwards, no other code may call any socket methods while these are running
						// Multiple threads calling into the same socket code can crash and corrupt state
						var readerTask = Task.Run(() => Reader(cancellation.Token), cancellation.Token);
						var writerTask = Task.Run(() => Writer(cancellation.Token), cancellation.Token);


						// re-join any channels that were already joined
						foreach (var channel in joinedChannels)
						{
							JoinRoom(channel);
						}

						var tasks = new[] { readerTask, writerTask, timeoutTask };
						var completedIdx = Task.WaitAny(tasks);
						// if WaitAny completed, it was due to an exception or cancellation

						var completedTaskStr = completedIdx switch
						{
							0 => "Reader",
							1 => "Writer",
							_ => "Timeout Monitor",
						};
						var e = tasks[completedIdx].Exception;
						Log.Warn(
							e != null
								? $"{completedTaskStr} failed with exception {e}"
								: $"{completedTaskStr} failed due to cancellation"
						);

						// kill other tasks by canceling the token
						cancellation.Cancel();
					}
					catch (WebSocketException we)
					{
						Log.Warn($"Unexpected websocket exception: {we.Message}");
						Log.Debug(we.StackTrace);
					}
					catch (ThreadAbortException)
					{
						Log.Warn("Thread aborted");
						throw;
					}
					catch (Exception e)
					{
						// TODO: maybe UI warn here?
						Log.Warn("An unexpected exception occurred");
						Log.Debug(e.GetType());
						Log.Warn(e.Message);
						Log.Debug(e.StackTrace);
						throw;
					}
				} while (attempts < maxAttempts);

				MainThreadScheduler.Schedule(
					static () =>
					{
						Log.Warn("An error occurred");
						DialogUtil.MakeDialog(
							STRINGS.ONITWITCH.UI.DIALOGS.CONNECTION_ERROR.TITLE,
							string.Format(STRINGS.ONITWITCH.UI.DIALOGS.CONNECTION_ERROR.BODY_FORMAT, maxAttempts),
							UI.CONFIRMDIALOG.OK,
							null
						);
					}
				);
			}
		);
	}

	public void JoinRoom([NotNull] string room)
	{
		// normalize room to #lowercase
		room = "#" + room.ToLowerInvariant().TrimStart('#');

		if (joinedChannels.Contains(room))
		{
			Log.Warn($"Trying to join already joined room {room}");
		}
		else
		{
			outgoingMessages.Enqueue(new IrcMessage(IrcCommandType.JOIN, new List<string> { room }));
		}
	}

	public void SendTextMessage([NotNull] string channel, [NotNull] string message)
	{
		channel = "#" + channel.ToLowerInvariant().TrimStart('#');
		var msg = new IrcMessage(IrcCommandType.PRIVMSG, new List<string> { channel, message });
		SendMessage(msg);
	}

	public void SendMessage(IrcMessage message)
	{
		outgoingMessages.Enqueue(message);
	}

	// Returns whether to pass the message on to listeners
	private bool HandleSystemMessage(IrcMessage message)
	{
		switch (message.Command.NumericId)
		{
			// ReSharper disable once CommentTypo
			//ERR_UNKNOWNCOMMAND
			case 421:
			{
				var command = message.Args[1];
				var reason = message.Args[2];
				Log.Debug($"Unknown command {command} ({reason})");
				return false;
			}
		}

		// We don't care about numeric messages or some other messages here
		// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
		switch (message.Command.Command)
		{
			case IrcCommandType.NOTICE:
				break;
			case IrcCommandType.PART:
			{
				var channel = message.Args[1];
				Log.Warn($"Forcibly disconnected from channel {channel} (banned)");
				break;
			}
			case IrcCommandType.PING:
			{
				Interlocked.Exchange(ref lastPingUtcTicks, System.DateTime.UtcNow.Ticks);
				outgoingMessages.Enqueue(new IrcMessage(IrcCommandType.PONG));
				return false;
			}
			case IrcCommandType.CAP:
			{
				Log.Debug($"Unexpected CAP message after login: {message}");
				return false;
			}
			case IrcCommandType.CLEARCHAT:
			case IrcCommandType.CLEARMSG:
			{
				// TODO: do we want to clear votes if this happens? Probably not. Consult streamers.
				break;
			}
			case IrcCommandType.RECONNECT:
			{
				Log.Warn("Twitch sent RECONNECT, connection will be dropped shortly.");
				return false;
			}
			case IrcCommandType.ROOMSTATE:
			{
				break;
			}
			case IrcCommandType.USERSTATE:
			{
				var channel = message.Args[0].ToLowerInvariant().TrimStart('#');
				if (joinedChannels.Add(channel))
				{
					Log.Info($"Joined channel {channel}");
				}

				break;
			}
		}

		return true;
	}

	private async Task Connect()
	{
		// how long before a single connection attempt should time out
		const int connectTimeout = 5000;

		// maximum number of attempts to make to connect to Twitch, backing off exponentially each time
		// this total time before final failure is about 45 seconds if the connection is failing instantly
		// or closer to 50 seconds if the final connection times out
		const int maxConnectAttempts = 8;
		for (var attempt = 0; attempt < maxConnectAttempts; attempt++)
		{
			try
			{
				Log.Debug($"Starting socket connection attempt {attempt + 1}");
				// a fresh socket has to be used every time
				socket = new ClientWebSocket();
				var tokenSource = new CancellationTokenSource(connectTimeout);
				await socket.ConnectAsync(TwitchChatUri, tokenSource.Token);
				Log.Debug($"Successfully connected to Twitch on attempt {attempt + 1}");
				break;
			}
			catch (Exception e)
			{
				Log.Debug($"Attempt {attempt + 1} failed: {e.Message}");
				// rethrow the final exception that occurs
				if (attempt == maxConnectAttempts - 1)
				{
					throw;
				}

				// we only expect WebSocketException here
				// TODO: test invalidated DNS cache and other failure modes
				if (e is not WebSocketException)
				{
					throw;
				}
			}

			const int maxBackoff = 15000;
			var delay = Mathf.Min(Mathf.FloorToInt(500 * Mathf.Pow(2, attempt)), maxBackoff);
			Log.Debug($"Delaying next connection attempt for {delay}ms");
			await Task.Delay(delay);
		}
	}

	// Requests capabilities from Twitch, authenticates, and waits for GLOBALUSERSTATE to confirm authentication
	private async Task Login(Credentials credentials)
	{
		Log.Debug("Requesting capabilities");
		await SendMessageImmediate(
			new IrcMessage(IrcCommandType.CAP, new List<string> { "REQ", "twitch.tv/commands twitch.tv/tags" })
		);

		Log.Debug("Authenticating");
		await SendMessageImmediate(new IrcMessage(IrcCommandType.PASS, new List<string> { credentials.Oauth }));
		await SendMessageImmediate(new IrcMessage(IrcCommandType.NICK, new List<string> { credentials.Nick }));

		// wait for GLOBALUSERSTATE, but only if the user is not anonymous
		if (credentials.Nick.StartsWith("justinfan", StringComparison.Ordinal))
		{
			Log.Debug($"Using anonymous login {credentials.Nick}, not waiting for authentication");
			return;
		}

		while (true)
		{
			var message = await GetMessage(CancellationToken.None);
			if (message.Command.Command == IrcCommandType.GLOBALUSERSTATE)
			{
				Log.Info(
					message.Tags.TryGetValue("display-name", out var name)
						? $"Logged into Twitch as {name.Value}"
						: "Logged into Twitch"
				);
				break;
			}
		}
	}

	private async Task Reader(CancellationToken cancellationToken)
	{
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var message = await GetMessage(cancellationToken);

			// don't pass on certain messages like PING
			if (HandleSystemMessage(message))
			{
				var onMessage = OnMessage;
				onMessage?.Invoke(message);
			}

			// parse privmsg and pass it to registered handlers
			if (message.Command.Command == IrcCommandType.PRIVMSG)
			{
				if (message.Args.Count < 2)
				{
					Log.Warn($"PRIVMSG did not include all args: {message}");
					return;
				}

				var msg = message.Args[1];
				var tags = message.Tags;

				var userId = tags["user-id"].Value;

				// use the display-name tag if it exists, otherwise use the message nick
				var displayName = message.Args[0].TrimStart('#');
				if (tags.TryGetValue("display-name", out var displayTag) && !string.IsNullOrEmpty(displayTag.Value))
				{
					displayName = displayTag.Value;
				}

				Color32? color = null;
				if (tags.TryGetValue("color", out var colorTag) && !string.IsNullOrEmpty(colorTag.Value))
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

				// make value copy so that it doesn't race with the null check
				var onMsg = OnChatMessage;
				onMsg?.Invoke(twitchMessage);
			}
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private async Task Writer(CancellationToken cancellationToken)
	{
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (outgoingMessages.TryDequeue(out var msg))
			{
				await SendMessageImmediate(msg);
			}
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private async Task SendMessageImmediate(IrcMessage message)
	{
		Log.Debug($"Sending message {message}");
		var str = message.GetIrcString();
		// BUG: a message that *contains* a \r\n is not well formed and not handled here
		if (!str.EndsWith("\r\n", StringComparison.Ordinal))
		{
			// remove all counts of either \r or \n and then re-add to normalize it
			str = str.TrimEnd('\r', '\n') + "\r\n";
		}

		var data = Encoding.UTF8.GetBytes(str);

		// any exceptions here should unwind to caller
		await socket.SendAsync(
			new ArraySegment<byte>(data),
			WebSocketMessageType.Text,
			true,
			CancellationToken.None
		);
	}

	private async Task<IrcMessage> GetMessage(CancellationToken cancellationToken)
	{
		if (incomingMessages.Count > 0)
		{
			return incomingMessages.Dequeue();
		}

		// receive messages until at least one is successfully queued
		do
		{
			cancellationToken.ThrowIfCancellationRequested();
			var buffer = new ArraySegment<byte>(new byte[8192]);
			using var s = new MemoryStream();
			WebSocketReceiveResult result;
			do
			{
				result = await socket.ReceiveAsync(buffer, cancellationToken);
				s.Write(buffer.Array!, buffer.Offset, result.Count);
			} while (result.EndOfMessage != true);

			var bytes = s.ToArray();
			var str = Encoding.UTF8.GetString(bytes);

			foreach (var message in str.Split('\n'))
			{
				var trimmed = message.TrimEnd('\r', '\n');
				if (string.IsNullOrWhiteSpace(trimmed))
				{
					continue;
				}

				var parsed = IrcParser.ParseMessage(trimmed);
				if (parsed.HasValue)
				{
					Log.Debug($"Received message {parsed.Value}");
					incomingMessages.Enqueue(parsed.Value);
				}
				else
				{
					Log.Warn($"Unable to parse IRC message {trimmed}");
				}
			}
		} while (incomingMessages.Count == 0);

		return incomingMessages.Dequeue();
	}
}
