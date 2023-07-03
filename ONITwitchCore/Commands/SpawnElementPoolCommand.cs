using System.Collections.Generic;
using System.Linq;
using ONITwitchLib.Utils;
using UnityEngine;
using ToastManager = ONITwitch.Toasts.ToastManager;

namespace ONITwitch.Commands;

internal class SpawnElementPoolCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);

	public override bool Condition(object data)
	{
		var pool = (Dictionary<string, float>) data;
		return pool.Any(entry => ElementUtil.ElementExistsAndEnabled(entry.Key));
	}

	private static readonly float UseInsulationThreshold =
		GameUtil.GetTemperatureConvertedToKelvin(200f, GameUtil.TemperatureUnit.Celsius);

	public override void Run(object data)
	{
		var pool = (Dictionary<string, float>) data;
		List<(Element Element, float Mass)> enabledElements = pool
			.Select(entry => (ElementUtil.FindElementByNameFast(entry.Key), entry.Value))
			.Where(entry => ElementUtil.ElementExistsAndEnabled(entry.Item1))
			.ToList();
		var selected = enabledElements.GetRandom();
		var insulationElement = ElementLoader.FindElementByHash(SimHashes.SuperInsulator);
		var defaultTemp = selected.Element.defaultValues.temperature;

		var mouseCell = PosUtil.RandomCellNearMouse();
		var cell = GridUtil.FindCellWithFoundationClearance(mouseCell);

		// middle tile always exists
		SimMessages.ReplaceAndDisplaceElement(
			cell,
			selected.Element.id,
			SpawnEvent,
			selected.Mass,
			defaultTemp
		);

		foreach (var neighborCell in GridUtil.GetNeighborsInBounds(cell))
		{
			// surround tiles with insulation if it's >200C

			if (defaultTemp > UseInsulationThreshold)
			{
				// Make sure that the target temp has at LEAST a 300 degree buffer if we're surrounding it in insulation
				var targetTemp = Mathf.Max(selected.Element.lowTemp + 300, defaultTemp);
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					insulationElement.id,
					SpawnEvent,
					float.Epsilon,
					targetTemp
				);
			}
			else
			{
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					selected.Element.id,
					SpawnEvent,
					selected.Mass,
					defaultTemp
				);
			}
		}

		ToastManager.InstantiateToastWithPosTarget(
			STRINGS.ONITWITCH.TOASTS.ELEMENT_GROUP.TITLE,
			string.Format(
				STRINGS.ONITWITCH.TOASTS.ELEMENT_GROUP.BODY_FORMAT,
				Util.StripTextFormatting(selected.Element.name)
			),
			Grid.CellToPos(cell)
		);
	}
}
