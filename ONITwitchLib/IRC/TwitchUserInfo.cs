using System.Diagnostics.CodeAnalysis;
using ONITwitchLib.Attributes;
using UnityEngine;

namespace ONITwitchLib.IRC;

/// <summary>
///     Information about a user, from their most recent chat message.  Primary key is the <paramref name="UserId" />.
/// </summary>
/// <param name="UserId">The Twitch-assigned stable ID for this user.</param>
/// <param name="DisplayName">The current display name.</param>
/// <param name="NameColor">The current name color, if it exists.</param>
/// <param name="IsModerator">Whether this user is a moderator.</param>
/// <param name="IsSubscriber">Whether this user is a subscriber.</param>
/// <param name="IsVip">Whether this user is a VIP.</param>
[NotPublicAPI]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public readonly record struct TwitchUserInfo(
	string UserId,
	string DisplayName,
	Color32? NameColor,
	bool IsModerator,
	bool IsSubscriber,
	bool IsVip
)
{
	/// <summary>
	///     Compares this <see cref="TwitchUserInfo" /> with another by their underlying <see cref="UserId" />.
	/// </summary>
	/// <param name="other">The other <see cref="TwitchUserInfo" /> to compare.</param>
	/// <returns>Whether the <see cref="UserId" /> of two <see cref="TwitchUserInfo" /> are equal.</returns>
	public bool Equals(TwitchUserInfo other)
	{
		return UserId == other.UserId;
	}

	/// <summary>
	///     Gets a hash code for this <see cref="TwitchUserInfo" /> by its <see cref="UserId" />.
	/// </summary>
	/// <returns>A hash code for the object.</returns>
	public override int GetHashCode()
	{
		return UserId != null ? UserId.GetHashCode() : 0;
	}
}
