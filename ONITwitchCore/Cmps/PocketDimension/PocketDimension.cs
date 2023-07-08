using System.Linq;
using HarmonyLib;
using KSerialization;
using ONITwitch.Content;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitch.Cmps.PocketDimension;

[SerializationConfig(MemberSerialization.OptIn)]
internal class PocketDimension : KMonoBehaviour, ISim200ms, ISim4000ms
{
	public const string BorderTemplate = "TwitchIntegration/PocketDimensionBorder";
	public const string MeepTemplate = "TwitchIntegration/MeepFace";
	public static readonly Vector2I DimensionSize = new(36, 38);
	public static readonly Vector2I InternalOffset = new(3, 3);
	public static readonly Vector2I InternalSize = new(30, 30);

	[Serialize] public Ref<PocketDimensionExteriorPortal> ExteriorPortal;

	// defaults to make sure that it doesn't think it's dead on spawn
	// ReSharper disable once InconsistentNaming
	[Serialize] public float Lifetime = 1;

	// ReSharper disable once InconsistentNaming
	[Serialize] public float MaxLifetime = 1;

#pragma warning disable 649
	[MyCmpGet] private WorldContainer world;
#pragma warning restore 649

	protected override void OnSpawn()
	{
		base.OnSpawn();

		if (world == null)
		{
			Log.Warn("Pocket Dimension was improperly deleted!");
			Destroy(gameObject);
			return;
		}

		var door = ExteriorPortal.Get();
		if (door != null)
		{
			// set parent to the world the door is in
			// This has to be here on spawn, because parent worlds aren't saved
			world.SetParentIdx(door.GetMyWorldId());

			WorldUtil.AddDiagnostic(
				world.id,
				new DimensionClosingDiagnostic(world.id)
			);
		}
		else
		{
			Log.Warn("no exterior door linked to pocket dimension");
			DestroyWorld();
		}
	}

	// The fraction of the lifetime remaining, clamped between 0 and 1
	private float GetFractionLifetimeRemaining()
	{
		return Mathf.Clamp01(Lifetime / MaxLifetime);
	}

	public void Sim200ms(float dt)
	{
		// SimXms calls still happen when the component is disabled, make sure to skip
		if (!enabled)
		{
			return;
		}

		if (Lifetime > 0)
		{
			Lifetime -= dt;
		}

		if (Lifetime <= 0)
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
			Log.Warn($"World selector did not have a row for world {world} (idx {world.id})");
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
			Log.Warn("Destroying world with no exterior door!");
		}

		if (world != null)
		{
			ClusterManager.Instance.SetActiveWorld(
				door != null ? door.GetMyWorldId() : ClusterManager.Instance.GetStartWorld().id
			);

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

			world.CancelChores();
			world.DestroyWorldBuildings(out _);

			if (door != null)
			{
				door.Destroy();
			}

			ExteriorPortal.Set(null);

			// unregister the world immediately to make sure that the world is inaccessible
			ClusterManager.Instance.UnregisterWorldContainer(world);

			// destroy all critters, so they don't pop out
			foreach (var pickupable in Components.Pickupables.GetWorldItems(world.id))
			{
				if (pickupable.TryGetComponent<CreatureBrain>(out _))
				{
					Destroy(pickupable.gameObject);
				}
			}

			// Wait to eject dupes until chores have been canceled so that dupes hopefully don't have the bug
			// where they pick up the old Y position
			GameScheduler.Instance.ScheduleNextFrame(
				"Finish Delete World",
				_ =>
				{
					world.EjectAllDupes(exitPos);
					foreach (var minionIdentity in Components.MinionIdentities.GetWorldItems(world.id))
					{
						minionIdentity.transform.SetPosition(exitPos);
					}

					WorldUtil.FreeGridSpace(world.WorldSize, world.WorldOffset);
					var trav = Traverse.Create(world);
					trav.Method("TransferPickupables", exitPos).GetValue();

					Destroy(world.gameObject);
				}
			);

			// disable the component, so it only destroys once
			enabled = false;
		}
	}
}
