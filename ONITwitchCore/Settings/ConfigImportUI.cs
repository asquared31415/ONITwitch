using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONITwitchCore.Config;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitchCore.Settings;

internal class ConfigImportUI : KScreen
{
	private TMP_InputField configInput;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();

		transform.Find("TitleBar/XButton").GetComponent<Button>().onClick.AddListener(() => Destroy(gameObject));
		configInput = HookUpInput("Content/MainInput/InputField");
		transform.Find("Content/EndButtonContainer/OKButton")
			.GetComponent<Button>()
			.onClick.AddListener(
				() =>
				{
					ProcessConfig(configInput.text ?? "");
					Destroy(gameObject);
				}
			);
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = true;

		base.OnSpawn();
	}

	private TMP_InputField HookUpInput(string basePath)
	{
		var inputGo = transform.Find(basePath).gameObject;
		var input = inputGo.AddOrGet<TMP_InputField>();
		input.textViewport = inputGo.transform.Find("Text Area").GetComponent<RectTransform>();
		input.textComponent = inputGo.transform.Find("Text Area/Text").GetComponent<TMP_Text>();

		return input;
	}

	private static void ProcessConfig([NotNull] string config)
	{
		try
		{
			// first decode from base64, this **requires** padding for some reason
			// padding should only be 0, 1, or 2 characters, never 3, but that would only happen if its already malformed
			// (4 - (len % 4)) % 4 because a multiple of 4 should have no change 
			config = config.PadRight(config.Length + (4 - config.Length % 4) % 4, '=');
			config = config.Replace('-', '+');
			config = config.Replace('_', '/');
			var bytes = Convert.FromBase64String(config);

			using var dataStream = new MemoryStream();
			// scope the compressor so that it flushes after writing, and make sure to leave the underlying output stream open
			using var inputStream = new MemoryStream(bytes);
			using (var decompressor = new DeflateStream(inputStream, CompressionMode.Decompress))
			{
				decompressor.CopyTo(dataStream);
			}

			// should be valid json at this point
			var decompressed = Encoding.UTF8.GetString(dataStream.ToArray());
			if (!decompressed.IsNullOrWhiteSpace())
			{
				var pretty = JToken.Parse(decompressed).ToString(Formatting.Indented);
				File.WriteAllText(UserCommandConfigManager.CommandConfigPath, pretty);
			}
			else
			{
				Log.Warn($"Invalid compressed data {config}");
				ShowError();
			}
		}
		catch (FormatException e)
		{
			Log.Warn($"Invalid base64 {config}");
			ShowError();
		}
	}

	private static void ShowError()
	{
		DialogUtil.MakeDialog("Invalid Config", "The config provided was invalid", "Ok", null);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			Destroy(gameObject);
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	// sort above the modal mods menu (100)
	public override float GetSortKey()
	{
		return 200;
	}
}
