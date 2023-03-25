using KSerialization;
using ONITwitchLib.Utils;

namespace ONITwitch.Cmps.PocketDimension;

[SerializationConfig(MemberSerialization.OptIn)]
internal class PocketDimensionInteriorPortal : KMonoBehaviour
{
	[Serialize] public Ref<PocketDimensionExteriorPortal> ExteriorPortal;

	protected override void OnSpawn()
	{
		base.OnSpawn();

		gameObject.GetComponent<KBatchedAnimController>().Play("idle", KAnim.PlayMode.Loop);

		GameScheduler.Instance.Schedule(
			"update pocket dim pathfinding",
			0.1f,
			_ =>
			{
				var building = gameObject.GetComponent<Building>();
				foreach (var cell in building.PlacementCells)
				{
					foreach (var c in GridUtil.GetNeighborsInBounds(cell))
					{
						Pathfinding.Instance.AddDirtyNavGridCell(c);
					}
				}
			}
		);
	}
}
