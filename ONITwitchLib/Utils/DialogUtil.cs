using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

/// <summary>
/// Methods to create pop up dialogs.
/// </summary>
[PublicAPI]
public static class DialogUtil
{
	/// <summary>
	/// Creates a dialog with only a confirm action.
	/// </summary>
	/// <param name="title">The title of the dialog.</param>
	/// <param name="text">The message in the body of the dialog.</param>
	/// <param name="confirmText">The text on the confirm button.</param>
	/// <param name="onConfirm">If not <see langword="null"/>, the action to call when the confirm button is pressed.</param>
	/// <returns>The newly created dialog.</returns>
	[PublicAPI]
	public static KScreen MakeDialog(
		string title,
		string text,
		string confirmText,
		[CanBeNull] System.Action onConfirm
	)
	{
		var screen = (ConfirmDialogScreen) KScreenManager.Instance.StartScreen(
			ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
			Global.Instance.globalCanvas
		);
		screen.PopupConfirmDialog(
			text,
			onConfirm,
			null,
			null,
			null,
			title,
			confirmText
		);
		return screen;
	}

	/// <summary>
	/// Creates a dialog with confirm, cancel, and optionally a third button.
	/// </summary>
	/// <param name="title">The title of the dialog.</param>
	/// <param name="text">The message in the body of the dialog.</param>
	/// <param name="confirmText">The text on the confirm button.</param>
	/// <param name="onConfirm">If not <see langword="null"/>, the action to call when the confirm button is pressed.</param>
	/// <param name="cancelText">The text on the cancel button.</param>
	/// <param name="onCancel">If not <see langword="null"/>, the action to call when the cancel button is pressed.</param>
	/// <param name="thirdText">The text on the third button.</param>
	/// <param name="thirdAction">If not <see langword="null"/>, the action to call when the third button is pressed.</param>
	/// <returns>The newly created dialog.</returns>
	[PublicAPI]
	public static KScreen MakeDialog(
		string title,
		string text,
		string confirmText,
		[CanBeNull] System.Action onConfirm,
		string cancelText,
		[CanBeNull] System.Action onCancel,
		string thirdText = null,
		[CanBeNull] System.Action thirdAction = null
	)
	{
		var screen = (ConfirmDialogScreen) KScreenManager.Instance.StartScreen(
			ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject,
			Global.Instance.globalCanvas
		);
		screen.PopupConfirmDialog(
			text,
			onConfirm,
			onCancel,
			thirdText,
			thirdAction,
			title,
			confirmText,
			cancelText
		);
		return screen;
	}
}
