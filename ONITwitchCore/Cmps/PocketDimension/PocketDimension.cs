using System.Linq;
using HarmonyLib;
using KSerialization;
using ONITwitchCore.Content;
using ONITwitchLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore.Cmps.PocketDimension;

[SerializationConfig(MemberSerialization.OptIn)]
public class PocketDimension : KMonoBehaviour, ISim200ms, ISim4000ms
{
	public const string BorderTemplate = "TwitchIntegration/PocketDimensionBorder";
	public const string MeepTemplate = "TwitchIntegration/MeepFace";
	public static readonly Vector2I DimensionSize = new(36, 38);
	public static readonly Vector2I InternalOffset = new(3, 3);
	public static readonly Vector2I InternalSize = new(30, 30);

	[Serialize] public Ref<PocketDimensionExteriorPortal> ExteriorPortal;

	// defaults to make sure that it doesn't think it's dead on spawn
	[Serialize] public float Lifetime = 1;
	[Serialize] public float MaxLifetime = 1;

#pragma warning disable 649
	[MyCmpGet] private WorldContainer world;
#pragma warning restore 649

	protected override void OnSpawn()
	{
		base.OnSpawn();

		var door = ExteriorPortal.Get();
		if (door != null)
		{
			// set parent to the world the door is in
			// This has to be here on spawn, because parent worlds aren't saved
			world.SetParentIdx(door.GetMyWorldId());
		}
		else
		{
			Debug.LogWarning("[Twitch Integration] no exterior door linked to pocket dimension");
			DestroyWorld();
		}

		WorldUtil.AddDiagnostic(
			world.id,
			new DimensionClosingDiagnostic(world.id)
		);
	}

	// The fraction of the lifetime remaining, clamped between 0 and 1
	public float GetFractionLifetimeRemaining()
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
			Debug.LogWarning(
				$"[Twitch Integration] World selector did not have a row for world {world} (idx {world.id})"
			);
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

			world.EjectAllDupes(exitPos);
			foreach (var minionIdentity in Components.MinionIdentities.GetWorldItems(world.id))
			{
				minionIdentity.transform.SetPosition(exitPos);
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

			GameScheduler.Instance.Schedule(
				"Finish Delete World",
				4f,
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
