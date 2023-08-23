using System;
using UnityEngine;

namespace ONITwitch.Content.Cmps.PocketDimension;

internal class PocketDimensionExteriorPortalSideScreen : KMonoBehaviour, ISidescreenButtonControl
{
#pragma warning disable CS0649
	[MyCmpReq] private PocketDimensionExteriorPortal exteriorPortal;
#pragma warning restore CS0649

	public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
	{
		throw new NotImplementedException();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		// Enable the button only when not on that world
		return ClusterManager.Instance.activeWorldId != exteriorPortal.CreatedWorldIdx;
	}

	public void OnSidescreenButtonPressed()
	{
		var portal = exteriorPortal.InteriorPortal.Get();

		// This has to be an if...else because these resolve to two different overloads with different behavior.
		// The overload that provides a position can't disable the position usage at all, that's
		// a private function.
		if (portal != null)
		{
			CameraController.Instance.ActiveWorldStarWipe(
				exteriorPortal.CreatedWorldIdx,
				portal.transform.position +
				(Vector3) (Vector2) (PocketDimension.InternalSize / 2)
			);
		}
		else
		{
			CameraController.Instance.ActiveWorldStarWipe(exteriorPortal.CreatedWorldIdx);
		}
	}

	public int HorizontalGroupID()
	{
		return -1;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 0;
	}

	public string SidescreenButtonText => STRINGS.ONITWITCH.UI.POCKET_DIMENSION_EXTERIOR_SIDE_SCREEN.NAME;
	public string SidescreenButtonTooltip => STRINGS.ONITWITCH.UI.POCKET_DIMENSION_EXTERIOR_SIDE_SCREEN.TOOLTIP;
}
