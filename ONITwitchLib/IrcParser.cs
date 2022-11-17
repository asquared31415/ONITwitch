using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ONITwitchLib;

public static class IrcParser
{
	// message must be a single message, it must not contain any CRLF
	// Returns the parsed message if it is valid
	public static IrcMessage? ParseMessage([NotNull] string message)
	{
		// This implementation is based on the details in RFC 1459: https://www.rfc-editor.org/rfc/rfc1459.html

		var parseMessage = string.Copy(message);
		var parsed = new IrcMessage();

		// parse tags if they exist
		if (parseMessage.StartsWith("@"))
		{
			var endTagIdx = parseMessage.IndexOf(' ');
			// don't include the @
			var tags = parseMessage.Substring(1, endTagIdx - 1);
			parseMessage = parseMessage.Substring(endTagIdx + 1);
			var parsedTags = ParseTags(tags);
			parsed.Tags = parsedTags;
		}

		// parse prefix if it exists
		if (parseMessage.StartsWith(":"))
		{
			// prefix continues from : to the next space
			var endPrefixIdx = parseMessage.IndexOf(' ');
			// don't include the leading : or trailing space
			parsed.Prefix = ParsePrefix(parseMessage.Substring(1, endPrefixIdx - 1));
			parseMessage = parseMessage.Substring(endPrefixIdx + 1);
		}

		// either params exist and they start with a space, or they do not exist and the command is
		// the rest of the message
		var endCommandIdx = parseMessage.IndexOf(' ');
		string commandStr;
		if (endCommandIdx == -1)
		{
			commandStr = parseMessage;
			parseMessage = "";
		}
		else
		{
			commandStr = parseMessage.Substring(0, endCommandIdx);
			parseMessage = parseMessage.Substring(endCommandIdx + 1);
		}

		var command = ParseCommand(commandStr);
		// command was invalid
		if (!command.HasValue)
		{
			return null;
		}

		parsed.Command = command.Value;

		var args = ParseArgs(parseMessage);
		parsed.Args = args;

		return parsed;
	}

	// NOTE: the regex for the vendor isn't quite right, but it's good enough
	private static readonly Regex TagRegex = new(
		@"^(?<client>\+)?((?<vendor>([a-zA-Z0-9-]*\.)*[a-zA-Z0-9-])\/)?(?<key_name>[a-zA-Z\d-]+)(=(?<value>[^\0\r\n;\ ]*))?$"
	);

	[NotNull]
	private static Dictionary<string, IrcTag> ParseTags([NotNull] string tags)
	{
		Debug.Log($"parsing tags:{tags}");
		var parseTags = string.Copy(tags);

		var tagDict = new Dictionary<string, IrcTag>();

		while (parseTags.Length > 0)
		{
			Debug.Log(parseTags);
			string fullTag;
			var tagEndIdx = parseTags.IndexOf(';');
			if (tagEndIdx == -1)
			{
				fullTag = parseTags;
				parseTags = "";
			}
			else
			{
				fullTag = parseTags.Substring(0, tagEndIdx);
				parseTags = parseTags.Substring(tagEndIdx + 1);
			}

			Debug.Log(fullTag);
			var match = TagRegex.Match(fullTag);
			var isClient = match.Groups["client"].Success;
			var vendor = match.Groups["vendor"].Value;
			var tagName = match.Groups["key_name"].Value;
			var tagValue = match.Groups["value"].Value;

			var tag = new IrcTag(isClient, vendor, tagName, tagValue);

			tagDict.Add(tagName, tag);
		}

		return tagDict;
	}

	private static string ParsePrefix([NotNull] string prefixString)
	{
		return prefixString;
	}

	// returns the command if it could be parsed
	[CanBeNull]
	private static IrcCommand? ParseCommand([NotNull] string command)
	{
		if (Regex.IsMatch(command, @"^[a-zA-z]+$"))
		{
			return command switch
			{
				"JOIN" => new IrcCommand(IrcCommandType.JOIN),
				"NICK" => new IrcCommand(IrcCommandType.NICK),
				"NOTICE" => new IrcCommand(IrcCommandType.NOTICE),
				"PART" => new IrcCommand(IrcCommandType.PART),
				"PASS" => new IrcCommand(IrcCommandType.PASS),
				"PING" => new IrcCommand(IrcCommandType.PING),
				"PONG" => new IrcCommand(IrcCommandType.PONG),
				"PRIVMSG" => new IrcCommand(IrcCommandType.PRIVMSG),
				"CAP" => new IrcCommand(IrcCommandType.CAP),
				"CLEARCHAT" => new IrcCommand(IrcCommandType.CLEARCHAT),
				"CLEARMSG" => new IrcCommand(IrcCommandType.CLEARMSG),
				"GLOBALUSERSTATE" => new IrcCommand(IrcCommandType.GLOBALUSERSTATE),
				"HOSTTARGET" => new IrcCommand(IrcCommandType.HOSTTARGET),
				"RECONNECT" => new IrcCommand(IrcCommandType.RECONNECT),
				"ROOMSTATE" => new IrcCommand(IrcCommandType.ROOMSTATE),
				"USERNOTICE" => new IrcCommand(IrcCommandType.USERNOTICE),
				"USERSTATE" => new IrcCommand(IrcCommandType.USERSTATE),
				"WHISPER" => new IrcCommand(IrcCommandType.WHISPER),
				_ => null,
			};
		}

		if (Regex.IsMatch(command, @"^[0-9]{3}$"))
		{
			var id = int.Parse(command);
			return new IrcCommand(id);
		}

		return null;
	}

	private static List<string> ParseArgs([NotNull] string args)
	{
		// some messages have no args
		if (args.Length == 0)
		{
			return new List<string>();
		}

		var argsList = new List<string>();

		var argsCopy = string.Copy(args);
		var foundFinalArg = false;
		while (!foundFinalArg && (argsCopy.Length > 0))
		{
			// parse last argument which may contain any characters except CRLF
			// but CRLF has already been stripped in processing
			if (argsCopy.StartsWith(":"))
			{
				argsList.Add(argsCopy.Substring(1));
				foundFinalArg = true;
			}
			else
			{
				var spaceIndex = argsCopy.IndexOf(" ", StringComparison.Ordinal);
				if (spaceIndex == -1)
				{
					Debug.LogWarning(
						$"[Twitch Integration] Parsing of IRC arguments \"{args}\" encountered end of string before expected"
					);
					return new List<string>();
				}

				var arg = argsCopy.Substring(0, spaceIndex);
				argsCopy = argsCopy.Substring(spaceIndex + 1);

				argsList.Add(arg);
			}
		}

		if (!foundFinalArg)
		{
			Debug.LogWarning($"[Twitch Integration] Parsing of IRC arguments \"{args}\" did not find the final colon");
			return new List<string>();
		}

		return argsList;
	}
}

public struct IrcMessage
{
	[NotNull] public Dictionary<string, IrcTag> Tags { get; internal set; }
	[CanBeNull] public string Prefix { get; internal set; }
	public IrcCommand Command { get; internal set; }
	[NotNull] public List<string> Args { get; internal set; }

	public IrcMessage()
	{
		Tags = new Dictionary<string, IrcTag>();
		Prefix = null;
		Command = default;
		Args = new List<string>();
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		if (Tags.Count > 0)
		{
			sb.Append("\nTags:\n");
			foreach (var (tagName, tag) in Tags)
			{
				sb.Append($"\t{tagName} {tag.Value}\n");
			}
		}

		sb.Append($"{Prefix} {Command} ");
		foreach (var arg in Args)
		{
			sb.Append(arg);
			sb.Append(" ");
		}

		return sb.ToString();
	}
}

public struct IrcCommand
{
	public bool IsNumeric { get; }

	// exactly one of either numericId or command will be set, depending on the value of IsNumeric
	public int? NumericId { get; }
	public IrcCommandType? Command { get; }

	public bool IsKnownCommand()
	{
		var isUnknown = IsNumeric && (NumericId == 421);
		return !isUnknown;
	}

	public IrcCommand(IrcCommandType command)
	{
		IsNumeric = false;
		Command = command;
	}

	public IrcCommand(int command)
	{
		IsNumeric = true;
		NumericId = command;
	}

	public override string ToString()
	{
		return IsNumeric ? NumericId.ToString() : Command.ToString();
	}
}

// ReSharper disable InconsistentNaming
public enum IrcCommandType
{
	JOIN,
	NICK,
	NOTICE,
	PART,
	PASS,
	PING,
	PONG,
	PRIVMSG,

	CAP,

	CLEARCHAT,
	CLEARMSG,
	GLOBALUSERSTATE,
	HOSTTARGET,
	RECONNECT,
	ROOMSTATE,
	USERNOTICE,
	USERSTATE,
	WHISPER,
}
// ReSharper restore InconsistentNaming

public struct IrcTag
{
	public bool IsClientTag { get; }
	[NotNull] public string TagName { get; }
	[CanBeNull] public string Vendor { get; }
	[CanBeNull] public string Value { get; }

	public IrcTag()
	{
		TagName = "INVALID";
	}

	public IrcTag([NotNull] string tagName)
	{
		TagName = tagName;
	}

	public IrcTag([NotNull] string tagName, string value)
	{
		TagName = tagName;
		Value = value;
	}

	public IrcTag(bool isClientTag, string vendor, [NotNull] string tagName, string value)
	{
		IsClientTag = isClientTag;
		Vendor = vendor;
		TagName = tagName;
		Value = value;
	}

	[NotNull]
	public string GetVendorNamespacedId()
	{
		return Vendor != null ? $"{Vendor}/{TagName}" : TagName;
	}
}