using TMPro;
using UnityEngine;

namespace ONITwitchCore.Toasts;

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
