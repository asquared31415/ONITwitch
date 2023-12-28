using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Content.Elements;

internal static class CustomElements
{
	[NotNull] [ItemNotNull] private static readonly List<ElementInfo> Elements =
	[
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
	];

	public static void RegisterSubstances([NotNull] List<Substance> substances)
	{
		substances.AddRange(Elements.Select(static ([NotNull] elementInfo) => elementInfo.CreateSubstance()));
	}
}
