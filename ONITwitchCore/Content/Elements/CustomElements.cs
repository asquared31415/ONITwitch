using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.Elements;

internal static class CustomElements
{
	[NotNull] [ItemNotNull] private static readonly List<ElementInfo> Elements = new()
	{
		new ElementInfo(
			nameof(TwitchSimHashes.OniTwitchIndestructibleElement),
			"TI_IndestructibleElement_kanim",
			Color.black,
			Element.State.Solid
		),

		new ElementInfo(
			nameof(TwitchSimHashes.OniTwitchSuperInsulator),
			"TI_SuperInsulator_kanim",
			Color.magenta,
			Element.State.Solid
		),
	};

	private static readonly AccessTools.FieldRef<object, Tag> SubstanceNameTagRef =
		AccessTools.FieldRefAccess<Tag>(typeof(Substance), "nameTag");

	public static void RegisterSubstances([NotNull] List<Substance> substances)
	{
		substances.AddRange(Elements.Select(static ([NotNull] elementInfo) => elementInfo.CreateSubstance()));
	}

	// TODO: (2023-09-10) Klei marked this bug as fixed in next update, check when that happens.
	public static void FixTags()
	{
		foreach (var elementInfo in Elements)
		{
			var element = ElementLoader.FindElementByHash(elementInfo.Hash);
			SubstanceNameTagRef(element.substance) = TagManager.Create(elementInfo.Id);
		}
	}
}
