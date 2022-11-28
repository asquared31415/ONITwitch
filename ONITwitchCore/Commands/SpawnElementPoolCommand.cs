using System.Collections.Generic;
using System.Linq;
using ONITwitchCore.Toasts;
using ONITwitchLib;
using UnityEngine;
using ToastManager = ONITwitchCore.Toasts.ToastManager;

namespace ONITwitchCore.Commands;

public class SpawnElementPoolCommand : CommandBase
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

	private static readonly float MaxAbyssaliteTemp =
		GameUtil.GetTemperatureConvertedToKelvin(600f, GameUtil.TemperatureUnit.Celsius);

	private static readonly float UseAbyssaliteThreshold =
		GameUtil.GetTemperatureConvertedToKelvin(200f, GameUtil.TemperatureUnit.Celsius);

	public override void Run(object data)
	{
		var pool = (List<string>) data;
		var enabledElements = pool.Select(ElementUtil.FindElementByNameFast)
			.Where(ElementUtil.ElementExistsAndEnabled)
			.ToList();
		var mouseCell = PosUtil.RandomCellNearMouse();
		var element = enabledElements.GetRandom();
		var abyssalite = ElementLoader.FindElementByHash(SimHashes.Katairite);

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

		// NOTE: when tuning these, make sure that the mass cannot cause pressure damage to the surrounding abyssalite
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
			// surround tiles with abyssalite if it's >200C

			// This uses the TARGET element's temperature, so the elements don't solidify
			// But also abyssalite can melt, so cap at 600C so that it's not too bad
			var targetTemp = Mathf.Clamp(
				element.defaultValues.temperature,
				0f,
				MaxAbyssaliteTemp
			);

			if (element.defaultValues.temperature > UseAbyssaliteThreshold)
			{
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					abyssalite.id,
					SpawnEvent,
					abyssalite.defaultValues.mass,
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
			"Element Created",
			$"{Util.StripTextFormatting(element.name)} was placed!",
			Grid.CellToPos(cell)
		);
	}
}
