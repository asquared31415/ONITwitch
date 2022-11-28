using System.Reflection;
using UnityEngine;

namespace ONITwitchCore;

public static class ModAssets
{
	private const string ToastManifestName = "ONITwitchCore.Resources.toast";

	public static class Toasts
	{
		public static GameObject ToastPrefab;
	}

	public static void LoadAssets()
	{
		var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ToastManifestName);
		if (assetStream == null)
		{
			Debug.LogWarning($"[Twitch Integration] Unable to locate embedded asset {ToastManifestName}");
		}

		var bundle = AssetBundle.LoadFromStream(assetStream);
		if (bundle == null)
		{
			Debug.LogWarning($"[Twitch Integration] Error loading asset bundle {ToastManifestName}");
		}

		Toasts.ToastPrefab = bundle.LoadAsset<GameObject>("assets/singletoast.prefab");
		Toasts.ToastPrefab.SetActive(false);
	}
}
