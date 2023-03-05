using ONITwitchCore.Cmps;
using ONITwitchCore.Toasts;

namespace ONITwitchCore.Commands;

public class EclipseCommand : CommandBase
{
	public override void Run(object data)
	{
		if ((Game.Instance != null) && Game.Instance.TryGetComponent<Eclipse>(out var eclipse))
		{
			var time = (float) (double) data;
			eclipse.StartEclipse(time);

			ToastManager.InstantiateToast(STRINGS.TOASTS.ECLIPSE.TITLE, STRINGS.TOASTS.ECLIPSE.BODY);
		}
	}
}
