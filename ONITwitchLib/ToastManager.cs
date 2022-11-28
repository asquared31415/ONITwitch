using System;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ONITwitchLib;

public static class ToastManager
{
	private static readonly Func<string, string, GameObject> InstantiateToastDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, GameObject>>(
			AccessTools.DeclaredMethod(EventInterface.ToastManagerType, "InstantiateToast"),
			null
		);

	private static readonly Func<string, string, Vector3, GameObject> InstantiateToastWithPosDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, Vector3, GameObject>>(
			AccessTools.DeclaredMethod(EventInterface.ToastManagerType, "InstantiateToastWithPosTarget"),
			null
		);

	private static readonly Func<string, string, GameObject, GameObject> InstantiateToastWithGoDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, GameObject, GameObject>>(
			AccessTools.DeclaredMethod(EventInterface.ToastManagerType, "InstantiateToastWithGoTarget"),
			null
		);

	[CanBeNull]
	public static GameObject InstantiateToast([CanBeNull] string title, [CanBeNull] string body)
	{
		return InstantiateToastDelegate(title, body);
	}

	[CanBeNull]
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos
	)
	{
		return InstantiateToastWithPosDelegate(title, body, pos);
	}

	[CanBeNull]
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target
	)
	{
		return InstantiateToastWithGoDelegate(title, body, target);
	}
}
