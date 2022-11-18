using UnityEngine;

namespace ONITwitchLib;

public record struct TwitchUserInfo(string UserId, string DisplayName, Color32? NameColor, bool IsModerator, bool IsSubscriber, bool IsVip);
