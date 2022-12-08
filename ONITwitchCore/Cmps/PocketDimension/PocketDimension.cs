using System.Linq;
using HarmonyLib;
using KSerialization;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore.Cmps.PocketDimension;

[SerializationConfig(MemberSerialization.OptIn)]
public class PocketDimension : KMonoBehaviour, ISim200ms, ISim4000ms
{
	public const string BorderTemplate = "TwitchIntegration/border";
	public const string MeepTemplate = "TwitchIntegration/MeepFace";
	public static readonly Vector2I DimensionSize = new(32, 32);
	public static readonly Vector2I InternalOffset = new(1, 1);
	public static readonly Vector2I InternalSize = new(30, 30);
	
	public const float MaxCyclesLifetime = 0.5f;

	[Serialize] public Ref<PocketDimensionExteriorPortal> ExteriorPortal;
	[Serialize] private float lifetime = MaxCyclesLifetime * Constants.SECONDS_PER_CYCLE;

#pragma warning disable 649
	[MyCmpGet] private WorldContainer world;
#pragma warning restore 649

	protected override void OnSpawn()
	{
		base.OnSpawn();

		// set parent to the world the door is in
		var door = ExteriorPortal.Get();
		if (door != null)
		{
			world.SetParentIdx(door.GetMyWorldId());
		}
		else
		{
			Debug.LogWarning("[Twitch Integration] no exterior door linked to pocket dimension");
			DestroyWorld();
		}

		/*
		WorldUtils.AddDiagnostic(
			world.id,
			new DimensionClosingDiagnostic(world.id)
		);
		*/
	}

	// The fraction of the lifetime remaining, clamped between 0 and 1
	public float GetFractionLifetimeRemaining()
	{
		return Mathf.Clamp01(lifetime / (MaxCyclesLifetime * Constants.SECONDS_PER_CYCLE));
	}

	public void Sim200ms(float dt)
	{
		// SimXms calls still happen when the component is disabled, make sure to skip
		if (!enabled)
		{
			return;
		}

		if (lifetime > 0)
		{
			lifetime -= dt;
		}

		if (lifetime <= 0)
		{
			DestroyWorld();
		}
	}

	// Update the fill amount of the UI element for the world
	public void Sim4000ms(float _)
	{
		// SimXms calls still happen when the component is disabled, make sure to skip
		if (!enabled || (world == null))
		{
			return;
		}

		if (!WorldSelector.Instance.worldRows.TryGetValue(world.id, out var worldRow))
		{
			Debug.LogWarning($"[Twitch Integration] World selector did not have a row for world {world} (idx {world.id})");
			return;
		}

		var hierarchy = worldRow.GetComponent<HierarchyReferences>();
		var image = hierarchy.GetReference<Image>("Icon");
		image.type = Image.Type.Filled;
		image.fillMethod = Image.FillMethod.Radial360;
		image.fillOrigin = (int) Image.Origin360.Top;
		image.fillClockwise = false;

		var clampedRatioRemaining = GetFractionLifetimeRemaining();
		image.fillAmount = clampedRatioRemaining;
	}

	public void DestroyWorld()
	{
		var door = ExteriorPortal.Get();
		if (door == null)
		{
			Debug.LogWarning($"[Twitch Integration] Destroying world with no exterior door!");
		}

		if (world != null)
		{
			if (ClusterManager.Instance.activeWorldId == world.id)
			{
				ClusterManager.Instance.SetActiveWorld(
					door != null ? door.GetMyWorldId() : ClusterManager.Instance.GetStartWorld().id
				);
			}

			Vector3 exitPos;
			if (door != null)
			{
				exitPos = Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(door.transform.position)));
			}
			else
			{
				var startWorld = ClusterManager.Instance.GetStartWorld();
				var telepad = Components.Telepads.GetWorldItems(startWorld.id).FirstOrDefault();
				exitPos = telepad != null
					? Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(telepad)))
					: (Vector3) (startWorld.minimumBounds + startWorld.maximumBounds) / 2;
			}

			world.EjectAllDupes(exitPos);
			world.CancelChores();
			world.DestroyWorldBuildings(out _);

			if (door != null)
			{
				door.Destroy();
			}

			ExteriorPortal.Set(null);

			// unregister the world immediately to make sure that the world is inaccessible
			ClusterManager.Instance.UnregisterWorldContainer(world);

			GameScheduler.Instance.ScheduleNextFrame(
				"Finish Delete World",
				_ =>
				{
					var trav = Traverse.Create(world);
					trav.Method("TransferPickupables", exitPos).GetValue();
					// actually deletes the world
					Traverse.Create(ClusterManager.Instance).Method("DeleteWorldObjects", world).GetValue();
					Destroy(world.gameObject);
				}
			);

			// disable the component, so it only destroys once
			enabled = false;
		}
		else
		{
			Debug.LogWarning(
				$"[Twitch Integration] Unable to destroy pocket dimension door: {door} world: {world}"
			);
		}
	}
}
