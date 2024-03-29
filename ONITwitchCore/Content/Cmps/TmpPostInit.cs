using TMPro;
using UnityEngine;

namespace ONITwitch.Content.Cmps;

internal class TmpPostInit : KMonoBehaviour
{
	[SerializeField] public TextAlignmentOptions alignment;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		var tmp = GetComponent<TMP_Text>();
		tmp.alignment = alignment;
	}
}
