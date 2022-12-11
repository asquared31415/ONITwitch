using System;
using System.Collections;
using KSerialization;
using OniTwitchLib;
using ONITwitchLib;
using ONITwitchLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore.Cmps;

[SerializationConfig(MemberSerialization.OptIn)]
public class Toast : KMonoBehaviour
{
	[NonSerialized] public float HoverTime = 15f;

	[NonSerialized] public FocusKind Focus;
	[NonSerialized] public Vector3 FocusPos;
	[NonSerialized] public GameObject FocusGo;

	[NonSerialized] public Text Body;
	[NonSerialized] public Text Title;

	[NonSerialized] private bool breakEarly;

	private const float AnimationTime = 0.5f;

	// TODO: calc better?
	private static readonly Vector2 StartPos = new(640, 128);
	private static readonly Vector2 EndPos = new(640, -20);

	public enum FocusKind
	{
		None,
		Position,
		Object,
	}

	protected override void OnPrefabInit()
	{
		breakEarly = false;

		var rect = GetComponent<RectTransform>();
		rect.anchoredPosition = StartPos;

		StartCoroutine(FadeInOut(StartPos, EndPos));
		var button = transform.Find("TargetButton").GetComponent<Button>();
		button.onClick.AddListener(OnClick);
		var closeButton = transform.Find("TitleContainer").Find("Close").Find("XButton").GetComponent<Button>();
		closeButton.onClick.AddListener(() => breakEarly = true);
	}

	private void OnClick()
	{
		switch (Focus)
		{
			case FocusKind.Position:
			{
				var position = FocusPos;
				position.z = -40;
				if (CameraController.Instance != null)
				{
					CameraController.Instance.SetTargetPos(position, 8f, true);
				}

				break;
			}
			case FocusKind.Object:
			{
				if (FocusGo != null)
				{
					if (CameraController.Instance != null)
					{
						CameraController.Instance.SetFollowTarget(FocusGo.transform);
					}
				}

				break;
			}
			case FocusKind.None:
			{
				break;
			}
			default:
				throw new ArgumentOutOfRangeException(nameof(Focus));
		}
	}

	private IEnumerator FadeInOut(Vector2 start, Vector2 end)
	{
		// HACK: Wait two extra frames to make sure we never get weirdness when being spawned on main menu
		yield return null;
		yield return null;

		var rect = GetComponent<RectTransform>();

		float elapsedTime = 0;
		while (elapsedTime <= AnimationTime)
		{
			var locStartPos = (Vector3) start;
			var locEndPos = (Vector3) end;
			rect.anchoredPosition = PosUtil.EaseOutLerp(
				locStartPos,
				locEndPos,
				elapsedTime / AnimationTime
			);

			elapsedTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitTimeOrSignal(HoverTime, () => breakEarly);

		elapsedTime = 0;
		while (elapsedTime <= AnimationTime)
		{
			var locStartPos = (Vector3) start;
			var locEndPos = (Vector3) end;
			// 1 - elapsedTime/seconds reverses the function
			rect.anchoredPosition = PosUtil.EaseOutLerp(
				locStartPos,
				locEndPos,
				1 - elapsedTime / AnimationTime
			);

			elapsedTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}

		Destroy(gameObject);
	}
}
