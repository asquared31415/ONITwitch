using System;

namespace ONITwitchLib.Attributes;

/// <summary>
/// Used to mark things as not part of the public API.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
[NotPublicAPI]
public class NotPublicAPIAttribute : Attribute
{
}
