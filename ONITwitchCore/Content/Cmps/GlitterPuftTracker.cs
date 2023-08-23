using KSerialization;
using ONITwitch.Integration.DecorPackA;
using ONITwitchLib.Logger;
using UnityEngine;

namespace ONITwitch.Content.Cmps;

// This is put on a bare GO directly and that GO should not have a SaveLoadRoot
// nothing should be saved
[SerializationConfig(MemberSerialization.OptIn)]
internal class GlitterPuftTracker : KMonoBehaviour
{
	// Note: layers [6, 31] are available for users
	// (U42-509629-S) Klei uses 6, 8-19, 22, 24-27, 29-31
	// The free layers are: 7, 20, 21, 23, 28
	public const int GlitterPuftLayerNumber = 21;
	private BoxCollider2D collider2D;

	private GlitterPuft thisPuft;

	// track other glitter pufts or mood lamps
	private void OnTriggerEnter2D(Collider2D other)
	{
		// if this is not a puft, don't try anything, it's just a marker
		if (thisPuft == null)
		{
			return;
		}

		if (other != collider2D)
		{
			if ((other.transform.parent.GetComponent<GlitterPuft>() != null) ||
				(other.transform.parent.GetComponent<OniTwitchGlitterMoodLampAccessor>() != null))
			{
				thisPuft.PuftGroup.Add(other.gameObject);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		// if this is not a puft, don't try anything, it's just a marker
		if (thisPuft == null)
		{
			return;
		}

		if (other != collider2D)
		{
			if ((other.transform.parent.GetComponent<GlitterPuft>() != null) ||
				(other.transform.parent.GetComponent<OniTwitchGlitterMoodLampAccessor>() != null))
			{
				if (!thisPuft.PuftGroup.Remove(other.gameObject))
				{
					Log.Debug("[Twitch Integration] Glitter Puft left collider without entering");
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();

		var go = gameObject;
		go.AddOrGet<Rigidbody2D>().isKinematic = true;

		collider2D = go.AddOrGet<BoxCollider2D>();
		collider2D.isTrigger = true;
		// 1.5 radius around
		collider2D.size = new Vector2(4, 4);
		// The default position is center bottom, so move it up so that the center is right
		collider2D.offset = new Vector2(0.0f, 0.5f);

		go.layer = GlitterPuftLayerNumber;

		thisPuft = go.transform.parent.GetComponent<GlitterPuft>();
	}
}
