using System;
using JetBrains.Annotations;

namespace ONITwitchLib.Attributes;

/// <summary>
///     Used to mark things as not part of the public API.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
[NotPublicAPI]
public class NotPublicAPIAttribute : Attribute
{
	// ReSharper disable once NotAccessedField.Local
	[NotNull] private readonly string note;

	[NotPublicAPI]
	internal NotPublicAPIAttribute()
	{
		note = "";
	}

	[NotPublicAPI]
	internal NotPublicAPIAttribute([NotNull] string note)
	{
		this.note = note;
	}
}
