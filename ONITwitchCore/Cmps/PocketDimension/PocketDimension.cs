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

#pragma warning disable 649
	[MyCmpGet] private WorldContainer world;
#pragma warning restore 649

	// defaults to make sure that it doesn't think it's dead on spawn
	[Serialize] public float Lifetime { get; private set; } = 1;
	[Serialize] public float MaxLifetime { get; private set; } = 1;

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

		UpdateImageTimer();
	}

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
			UpdateImageTimer();
		}
		else
		{
			Log.Warn("no exterior door linked to pocket dimension");
			DestroyWorld();
		}
	}

	public void SetLifetime(float newLifetime, float newMaxLifetime)
	{
		Lifetime = newLifetime;
		MaxLifetime = newMaxLifetime;
		UpdateImageTimer();
	}

	private void UpdateImageTimer()
	{
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

	// The fraction of the lifetime remaining, clamped between 0 and 1
	private float GetFractionLifetimeRemaining()
	{
		return Mathf.Clamp01(Lifetime / MaxLifetime);
	}

	public void DestroyWorld()
	{
		Log.Debug("Destroying Pocket Dimension");
		var door = ExteriorPortal.Get();
		if (door == null)
		{
			Log.Warn("Destroying world with no exterior door!");
		}

		if (world != null)
		{
			var targetWorldId = door != null ? door.GetMyWorldId() : ClusterManager.Instance.GetStartWorld().id;
			ClusterManager.Instance.SetActiveWorld(targetWorldId);

			Vector3 exitPos;
			if (door != null)
			{
				exitPos = Grid.CellToPosCCC(
					Grid.CellAbove(Grid.PosToCell(door.transform.position)),
					Grid.SceneLayer.Move
				);
			}
			else
			{
				var startWorld = ClusterManager.Instance.GetStartWorld();
				var telepad = Components.Telepads.GetWorldItems(startWorld.id).FirstOrDefault();
				exitPos = telepad != null
					? Grid.CellToPosCCC(Grid.PosToCell(telepad), Grid.SceneLayer.Move)
					: (Vector3) (startWorld.minimumBounds + startWorld.maximumBounds) / 2;
			}

			world.CancelChores();
			// Also clears zones
			world.DestroyWorldBuildings(out _);

			if (door != null)
			{
				door.Destroy();
			}

			ExteriorPortal.Set(null);

			// destroy all critters, so they don't pop out
			// TODO: some creatures shouldn't be destroyed. Need to add a way for other code to hook destruction on their prefabs.
			foreach (var pickupable in Components.Pickupables.GetWorldItems(world.id))
			{
				if (pickupable.TryGetComponent<CreatureBrain>(out _))
				{
					Destroy(pickupable.gameObject);
				}
			}

			// Workaround for dupes sometimes being teleported to the wrong position, interrupt them with a emote chore
			// and then teleport them next frame. Wait at least 33ms after teleporting to destroy the world, since the
			// FallMonitor only updates every 33ms.
			// It's unclear if this actually helps, because reproducing is a pain.
			var db = Db.Get();
			foreach (var minionIdentity in Components.LiveMinionIdentities.GetWorldItems(world.id))
			{
				_ = new EmoteChore(
					minionIdentity.GetComponent<ChoreProvider>(),
					db.ChoreTypes.EmoteHighPriority,
					db.Emotes.Minion.Cheer
				);

				if (minionIdentity.TryGetComponent(out ChoreDriver driver))
				{
					driver.StopChore();
				}
			}

			GameScheduler.Instance.ScheduleNextFrame(
				"TwitchIntegration.EjectDupes",
				_ =>
				{
					Log.Debug("Ejecting dupes from pocket dimension");
					world.EjectAllDupes(exitPos);

					GameScheduler.Instance.Schedule(
						"TwitchIntegration.FinishDestroyWorld",
						// at least 33ms to let fallers run
						0.066f,
						_ =>
						{
							Log.Debug("Destroying Pocket Dimension grid space");
							WorldUtil.FreeGridSpace(world.WorldSize, world.WorldOffset);

							Log.Debug("Unregistering Pocket Dimension");
							ClusterManager.Instance.UnregisterWorldContainer(world);
							var trav = Traverse.Create(world);
							trav.Method("TransferPickupables", exitPos).GetValue();

							Log.Debug("Deleting Pocket Dimension object");
							Destroy(world.gameObject);

							// Update all the dupe name displays again because the game doesn't update this when a dupe changes worlds.
							if (NameDisplayScreen.Instance != null)
							{
								Traverse.Create(NameDisplayScreen.Instance)
									.Method("OnActiveWorldChanged", new[] { typeof(object) })
									// Note: Klei Tuple not System Tuple.
									// This is not used by the method, but it's probably less likely to break if we pass it?
									.GetValue(new Tuple<int, int>(targetWorldId, targetWorldId));
							}

							GameScheduler.Instance.ScheduleNextFrame(
								"TwitchIntegration.PocketDimDirtyNav",
								_ =>
								{
									// Stop dupes from falling forever
									Log.Debug("dirty nav grid");
									Pathfinding.Instance.AddDirtyNavGridCell(Grid.PosToCell(exitPos));
								}
							);
						}
					);
				}
			);

			// disable the component, so it only destroys once
			enabled = false;
		}
	}
}
