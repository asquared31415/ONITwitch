using System;

namespace ONITwitchCore.Cmps.PocketDimension;

internal class PocketDimensionInteriorDoorSideScreen : KMonoBehaviour, ISidescreenButtonControl
{
#pragma warning disable CS0649
	[MyCmpReq] private PocketDimensionInteriorPortal interiorPortal;
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
		var portal = interiorPortal.ExteriorPortal.Get();
		// Enable the button only when not on that world
		return ClusterManager.Instance.activeWorldId != (portal != null
			? portal.GetMyWorldId()
			: ClusterManager.Instance.GetStartWorld().id);
	}

	public void OnSidescreenButtonPressed()
	{
		var portal = interiorPortal.ExteriorPortal.Get();

		// This has to be an if...else because these resolve to two different overloads with different behavior.
		// The overload that provides a position can't disable the position usage at all, that's
		// a private function.
		if (portal != null)
		{
			CameraController.Instance.ActiveWorldStarWipe(portal.GetMyWorldId(), portal.transform.position);
		}
		else
		{
			CameraController.Instance.ActiveWorldStarWipe(ClusterManager.Instance.GetStartWorld().id);
		}
	}

	public int HorizontalGroupID()
	{
		// TODO: what is this
		return -1;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 0;
	}

	public string SidescreenButtonText => STRINGS.ONITWITCH.UI.POCKET_DIMENSION_INTERIOR_SIDE_SCREEN.NAME;
	public string SidescreenButtonTooltip => STRINGS.ONITWITCH.UI.POCKET_DIMENSION_INTERIOR_SIDE_SCREEN.TOOLTIP;
}
