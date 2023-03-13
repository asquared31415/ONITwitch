using System.Collections.Generic;
using System.Linq;
using ONITwitchLib.Utils;
using UnityEngine;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

internal class SpawnElementPoolCommand : CommandBase
{
	private static readonly CellElementEvent SpawnEvent = new(
		"TwitchSpawnedElement",
		"Spawned by Twitch",
		true
	);

	public override bool Condition(object data)
	{
		var pool = (List<string>) data;
		return pool.Select(ElementUtil.FindElementByNameFast).Any(ElementUtil.ElementExistsAndEnabled);
	}

	private static readonly float UseInsulationThreshold =
		GameUtil.GetTemperatureConvertedToKelvin(200f, GameUtil.TemperatureUnit.Celsius);

	public override void Run(object data)
	{
		var pool = (List<string>) data;
		var enabledElements = pool.Select(ElementUtil.FindElementByNameFast)
			.Where(ElementUtil.ElementExistsAndEnabled)
			.ToList();
		var mouseCell = PosUtil.RandomCellNearMouse();
		var element = enabledElements.GetRandom();
		var insulation = ElementLoader.FindElementByHash(SimHashes.SuperInsulator);

		const float gasMass = 50f;
		const float liquidMass = 500f;
		const float solidMass = 1000f;

		var mass = (element.state & Element.State.Solid) switch
		{
			Element.State.Gas => gasMass,
			Element.State.Liquid => liquidMass,
			Element.State.Solid => solidMass,
			_ => 0f,
		};

		// NOTE: when tuning these, make sure that the mass cannot cause pressure damage to the surrounding tiles
		// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
		// ReSharper disable once ConvertSwitchStatementToSwitchExpression
		switch (element.id)
		{
			case SimHashes.MoltenGlass:
				mass = 1_000f;
				break;
			case SimHashes.Magma:
				mass = 1_000f;
				break;
		}

		var cell = GridUtil.FindCellWithFoundationClearance(mouseCell);

		// middle tile always exists
		SimMessages.ReplaceAndDisplaceElement(
			cell,
			element.id,
			SpawnEvent,
			mass,
			element.defaultValues.temperature
		);

		foreach (var neighborCell in GridUtil.GetNeighborsInBounds(cell))
		{
			// surround tiles with insulation if it's >200C

			// This uses the TARGET element's temperature, so the elements don't solidify
			var targetTemp = Mathf.Clamp(
				element.defaultValues.temperature,
				0f,
				9_999f
			);

			if (element.defaultValues.temperature > UseInsulationThreshold)
			{
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					insulation.id,
					SpawnEvent,
					float.Epsilon,
					targetTemp
				);
			}
			else
			{
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					element.id,
					SpawnEvent,
					mass,
					element.defaultValues.temperature
				);
			}
		}

		ToastManager.InstantiateToastWithPosTarget(
			STRINGS.ONITWITCH.TOASTS.ELEMENT_GROUP.TITLE,
			string.Format(
				STRINGS.ONITWITCH.TOASTS.ELEMENT_GROUP.BODY_FORMAT,
				Util.StripTextFormatting(element.name)
			),
			Grid.CellToPos(cell)
		);
	}
}
