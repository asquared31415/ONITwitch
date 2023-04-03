using ONITwitch.Cmps;
using ONITwitch.Toasts;
using ONITwitchLib.Logger;

namespace ONITwitch.Commands;

internal class EclipseCommand : CommandBase
{
	public override void Run(object data)
	{
		if ((Game.Instance != null) && Game.Instance.TryGetComponent<OniTwitchEclipse>(out var eclipse))
		{
			var time = (float) (double) data;
			eclipse.StartEclipse(time);

			ToastManager.InstantiateToast(STRINGS.ONITWITCH.TOASTS.ECLIPSE.TITLE, STRINGS.ONITWITCH.TOASTS.ECLIPSE.BODY);
		}
		else
		{
			Log.Warn("Unable to start eclipse");
		}
	}
}
