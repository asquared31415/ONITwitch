using UnityEngine;

namespace ONITwitchLib;

/// <summary>
/// Information about a user, from their most recent chat message.  Primary key is the <paramref name="UserId"/>.
/// </summary>
/// <param name="UserId">The Twitch-assigned stable ID for this user</param>
/// <param name="DisplayName">The current display name</param>
/// <param name="NameColor">The current name color, if it exists</param>
/// <param name="IsModerator">Whether this user is a moderator</param>
/// <param name="IsSubscriber">Whether this user is a subscriber</param>
/// <param name="IsVip">Whether this user is a VIP</param>
public record struct TwitchUserInfo(
	string UserId,
	string DisplayName,
	Color32? NameColor,
	bool IsModerator,
	bool IsSubscriber,
	bool IsVip
)
{
	public readonly bool Equals(TwitchUserInfo other)
	{
		return UserId == other.UserId;
	}

	public readonly override int GetHashCode()
	{
		return UserId != null ? UserId.GetHashCode() : 0;
	}
}
