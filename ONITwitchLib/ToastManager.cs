using System;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Core;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitchLib;

/// <summary>
///     Provides methods for creating toasts.
/// </summary>
[PublicAPI]
public static class ToastManager
{
	private static readonly Func<string, string, GameObject> InstantiateToastDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, GameObject>>(
			AccessTools.DeclaredMethod(
				CoreTypes.ToastManagerType,
				"InstantiateToast",
				new[] { typeof(string), typeof(string) }
			),
			null
		);

	private static readonly Func<string, string, Vector3, GameObject> InstantiateToastWithPosDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, Vector3, GameObject>>(
			AccessTools.DeclaredMethod(
				CoreTypes.ToastManagerType,
				"InstantiateToastWithPosTarget",
				new[] { typeof(string), typeof(string), typeof(Vector3) }
			),
			null
		);

	private static readonly Func<string, string, Vector3, float, GameObject> InstantiateToastWithPosAndSizeDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, Vector3, float, GameObject>>(
			AccessTools.DeclaredMethod(
				CoreTypes.ToastManagerType,
				"InstantiateToastWithPosTargetAndZoom",
				new[] { typeof(string), typeof(string), typeof(Vector3), typeof(float) }
			),
			null
		);

	private static readonly Func<string, string, GameObject, GameObject> InstantiateToastWithGoDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, GameObject, GameObject>>(
			AccessTools.DeclaredMethod(
				CoreTypes.ToastManagerType,
				"InstantiateToastWithGoTarget",
				new[] { typeof(string), typeof(string), typeof(GameObject) }
			),
			null
		);


	private static readonly Func<string, string, GameObject, float, GameObject> InstantiateToastWithGoAndSizeDelegate =
		DelegateUtil.CreateDelegate<Func<string, string, GameObject, float, GameObject>>(
			AccessTools.DeclaredMethod(
				CoreTypes.ToastManagerType,
				"InstantiateToastWithGoTargetAndZoom",
				new[] { typeof(string), typeof(string), typeof(GameObject), typeof(float) }
			),
			null
		);

	/// <summary>
	///     Creates a toast with a tile and a body.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToast([CanBeNull] string title, [CanBeNull] string body)
	{
		return InstantiateToastDelegate(title, body);
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that targets a position when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="pos">The position to target on click.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos
	)
	{
		return InstantiateToastWithPosDelegate(title, body, pos);
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that targets a position when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="pos">The position to target on click.</param>
	/// <param name="orthographicSize">
	///     The orthographic size the camera should go to. Higher is more zoomed out. Must be
	///     strictly greater than 0.
	/// </param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithPosTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		Vector3 pos,
		float orthographicSize
	)
	{
		return InstantiateToastWithPosAndSizeDelegate(title, body, pos, orthographicSize);
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that selects a <see cref="GameObject" /> when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="target">The <see cref="GameObject" /> to target on click.</param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target
	)
	{
		return InstantiateToastWithGoDelegate(title, body, target);
	}

	/// <summary>
	///     Creates a toast with a tile and a body, that selects a <see cref="GameObject" /> when clicked.
	/// </summary>
	/// <param name="title">The title text for the toast.</param>
	/// <param name="body">The body text for the toast.</param>
	/// <param name="target">The <see cref="GameObject" /> to target on click.</param>
	/// <param name="orthographicSize">
	///     The orthographic size the camera should go to. Higher is more zoomed out. Must be
	///     strictly greater than 0.
	/// </param>
	/// <returns>The newly created toast's <see cref="GameObject" />.</returns>
	[PublicAPI]
	[CanBeNull]
	public static GameObject InstantiateToastWithGoTarget(
		[CanBeNull] string title,
		[CanBeNull] string body,
		GameObject target,
		float orthographicSize
	)
	{
		return InstantiateToastWithGoAndSizeDelegate(title, body, target, orthographicSize);
	}
}
