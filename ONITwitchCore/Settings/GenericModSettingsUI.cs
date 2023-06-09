using System.Globalization;
using ONITwitch.Config;
using ONITwitch.Voting;
using ONITwitchLib;
using ONITwitchLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONITwitch.Settings;

internal class GenericModSettingsUI : KScreen
{
	private TMP_InputField channelInput;
	private TMP_InputField voteDelayInput;
	private TMP_InputField voteTimeInput;
	private TMP_InputField voteCountInput;
	private Toggle useTwitchNameColorsToggle;
	private Toggle showToastsToggle;
	private Toggle showVoteChoicesToggle;
	private Slider minDangerSlider;
	private Slider maxDangerSlider;

	private bool hasUnsavedChanges;
	private bool confirmExitDialogActive;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();

		ConsumeMouseScroll = true;

		transform.Find("TitleBar/XButton").GetComponent<Button>().onClick.AddListener(CloseMenu);

		channelInput = HookUpInput("Content/ChannelName/InputField");
		// https://discuss.dev.twitch.tv/t/max-length-for-user-names-and-display-names/21315
		channelInput.characterLimit = 25;
		channelInput.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip("Content/ChannelName/Label", STRINGS.ONITWITCH.UI.CONFIG.CHANNEL_NAME.TOOLTIP);

		voteDelayInput = HookUpInput("Content/VoteDelay/InputField");
		voteDelayInput.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip("Content/VoteDelay/Label", STRINGS.ONITWITCH.UI.CONFIG.TIME_BETWEEN_VOTES.TOOLTIP);

		voteTimeInput = HookUpInput("Content/VoteTime/InputField");
		voteTimeInput.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip("Content/VoteTime/Label", STRINGS.ONITWITCH.UI.CONFIG.VOTING_TIME.TOOLTIP);

		voteCountInput = HookUpInput("Content/VoteCount/InputField");
		voteCountInput.characterLimit = 3;
		voteCountInput.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip("Content/VoteCount/Label", STRINGS.ONITWITCH.UI.CONFIG.OPTIONS_PER_VOTE.TOOLTIP);

		useTwitchNameColorsToggle = transform.Find("Content/UseTwitchColors").GetComponent<Toggle>();
		useTwitchNameColorsToggle.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip("Content/UseTwitchColors/Label", STRINGS.ONITWITCH.UI.CONFIG.USE_TWITCH_COLORS.TOOLTIP);

		showToastsToggle = transform.Find("Content/ShowToasts").GetComponent<Toggle>();
		showToastsToggle.onValueChanged.AddListener(
			active =>
			{
				showVoteChoicesToggle.interactable = active;

				if (!active)
				{
					showVoteChoicesToggle.isOn = false;
				}

				hasUnsavedChanges = true;
			}
		);

		AddToolTip("Content/ShowToasts/Label", STRINGS.ONITWITCH.UI.CONFIG.SHOW_TOASTS.TOOLTIP);

		showVoteChoicesToggle = transform.Find("Content/SubToastSettings/ShowVoteStartToast").GetComponent<Toggle>();
		showVoteChoicesToggle.onValueChanged.AddListener(_ => hasUnsavedChanges = true);

		AddToolTip(
			"Content/SubToastSettings/ShowVoteStartToast/Label",
			STRINGS.ONITWITCH.UI.CONFIG.SHOW_START_TOASTS.TOOLTIP
		);

		minDangerSlider = transform.Find("Content/MinDangerInput/SliderContainer/Slider").GetComponent<Slider>();
		AddToolTip("Content/MinDangerInput/Label", STRINGS.ONITWITCH.UI.CONFIG.MIN_DANGER.TOOLTIP);

		maxDangerSlider = transform.Find("Content/MaxDangerInput/SliderContainer/Slider").GetComponent<Slider>();
		AddToolTip("Content/MaxDangerInput/Label", STRINGS.ONITWITCH.UI.CONFIG.MAX_DANGER.TOOLTIP);

		minDangerSlider.onValueChanged.AddListener(
			newVal =>
			{
				hasUnsavedChanges = true;
				if (newVal > maxDangerSlider.value)
				{
					maxDangerSlider.value = newVal;
				}
			}
		);
		maxDangerSlider.onValueChanged.AddListener(
			newVal =>
			{
				hasUnsavedChanges = true;
				if (newVal < minDangerSlider.value)
				{
					minDangerSlider.value = newVal;
				}
			}
		);

		transform.Find("Content/EditConfig/EditConfigButton")
			.GetComponent<Button>()
			.onClick.AddListener(
				() =>
				{
					UserCommandConfigManager.OpenConfigEditor();

					var config = Util.KInstantiateUI(ModAssets.Options.ConfigPopup, canvas.gameObject);
					config.AddOrGet<ConfigImportUI>();
					config.SetActive(true);
				}
			);
		AddToolTip("Content/EditConfig/EditConfigButton", STRINGS.ONITWITCH.UI.CONFIG.EDIT.TOOLTIP);


		transform.Find("Buttons/Version/VersionText").GetComponent<LocText>().text = "v" + Global.Instance.modManager
			.mods
			.Find(mod => mod.staticID == TwitchModInfo.StaticID)
			.packagedModInfo
			.version;
		transform.Find("Buttons/Github")
			.GetComponent<Button>()
			.onClick.AddListener(() => Application.OpenURL("https://github.com/asquared31415/ONI-mods"));
		transform.Find("Buttons/Steam")
			.GetComponent<Button>()
			.onClick.AddListener(
				() => Application.OpenURL("https://steamcommunity.com/id/asquared31415/myworkshopfiles/?appid=457140")
			);
		transform.Find("Buttons/OKButton").GetComponent<Button>().onClick.AddListener(ApplySettings);
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = true;

		base.OnSpawn();

		channelInput.text = GenericModSettings.Data.ChannelName;
		voteDelayInput.text = GenericModSettings.Data.VoteDelay.ToString(CultureInfo.CurrentCulture);
		voteTimeInput.text = GenericModSettings.Data.VoteTime.ToString(CultureInfo.CurrentCulture);
		voteCountInput.text = GenericModSettings.Data.VoteCount.ToString();
		useTwitchNameColorsToggle.isOn = GenericModSettings.Data.UseTwitchNameColors;

		showVoteChoicesToggle.isOn = GenericModSettings.Data.ShowVoteStartToasts;
		// set the main toggle last so that it can override the active state of the sub option(s)
		showToastsToggle.isOn = GenericModSettings.Data.ShowToasts;

		minDangerSlider.value = (float) GenericModSettings.Data.MinDanger;
		maxDangerSlider.value = (float) GenericModSettings.Data.MaxDanger;

		// writing the old values would set this to true, reset it
		hasUnsavedChanges = false;
	}

	private void CloseMenu()
	{
		if (hasUnsavedChanges)
		{
			confirmExitDialogActive = true;
			DialogUtil.MakeDialog(
				STRINGS.ONITWITCH.UI.DIALOGS.UNSAVED_CONFIG.TITLE,
				STRINGS.ONITWITCH.UI.DIALOGS.UNSAVED_CONFIG.BODY,
				STRINGS.ONITWITCH.UI.DIALOGS.UNSAVED_CONFIG.SAVE,
				() =>
				{
					ApplySettings();
					confirmExitDialogActive = false;
				},
				STRINGS.ONITWITCH.UI.DIALOGS.UNSAVED_CONFIG.BACK,
				() => { confirmExitDialogActive = false; },
				((string) STRINGS.ONITWITCH.UI.DIALOGS.UNSAVED_CONFIG.DISCARD).Colored(ColorUtil.RedWarningColor),
				() =>
				{
					Destroy(gameObject);
					confirmExitDialogActive = false;
				}
			);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private TMP_InputField HookUpInput(string basePath)
	{
		var inputGo = transform.Find(basePath).gameObject;
		var input = inputGo.AddOrGet<TMP_InputField>();
		input.textViewport = inputGo.transform.Find("Text Area").GetComponent<RectTransform>();
		input.textComponent = inputGo.transform.Find("Text Area/Text").GetComponent<TMP_Text>();

		return input;
	}

	private void AddToolTip(string path, string tooltipStr)
	{
		var toolTip = transform.Find(path).gameObject.AddOrGet<ToolTip>();
		toolTip.tooltipPivot = new Vector2(0.5f, 0f);
		toolTip.tooltipPositionOffset = new Vector2(toolTip.WrapWidth / 2f, 0f);
		toolTip.parentPositionAnchor = new Vector2(0f, 0f);

		ToolTipScreen.Instance.SetToolTip(toolTip);
		toolTip.SetSimpleTooltip(tooltipStr);
	}

	private void ApplySettings()
	{
		var defaultSettings = new GenericModSettings.SettingsData();

		if ((GenericModSettings.Data.MaxDanger < Danger.Deadly) && ((Danger) maxDangerSlider.value >= Danger.Deadly))
		{
			DialogUtil.MakeDialog(
				STRINGS.ONITWITCH.UI.DIALOGS.DEADLY_CONFIG.TITLE,
				STRINGS.ONITWITCH.UI.DIALOGS.DEADLY_CONFIG.BODY,
				STRINGS.ONITWITCH.UI.DIALOGS.DEADLY_CONFIG.CONTINUE,
				() => { }
			);
		}

		// updates and saves the config
		GenericModSettings.Data = new GenericModSettings.SettingsData
		{
			Version = GenericModSettings.CurrentConfigVersion,
			ChannelName = channelInput.text ?? defaultSettings.ChannelName,
			VoteDelay = float.TryParse(voteDelayInput.text, out var voteDelay) ? voteDelay : defaultSettings.VoteDelay,
			VoteTime = float.TryParse(voteTimeInput.text, out var voteTime) ? voteTime : defaultSettings.VoteTime,
			VoteCount = int.TryParse(voteCountInput.text, out var voteCount)
				? Mathf.Clamp(voteCount, 1, 5)
				: defaultSettings.VoteCount,
			UseTwitchNameColors = useTwitchNameColorsToggle.isOn,
			ShowToasts = showToastsToggle.isOn,
			ShowVoteStartToasts = showVoteChoicesToggle.isOn,
			MinDanger = (Danger) minDangerSlider.value,
			MaxDanger = (Danger) maxDangerSlider.value,
		};

		if (VoteController.Instance != null)
		{
			VoteController.Instance.JoinRoom(GenericModSettings.Data.ChannelName);
		}

		hasUnsavedChanges = false;
		CloseMenu();
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!confirmExitDialogActive && (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight)))
		{
			CloseMenu();
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
