using UnityEngine;

namespace ONITwitchLib;

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
