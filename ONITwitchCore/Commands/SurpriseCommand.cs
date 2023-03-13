using EventLib;
using ONITwitchLib.Logger;

namespace ONITwitchCore.Commands;

internal class SurpriseCommand : CommandBase
{
	public override void Run(object data)
	{
		EventInfo info;
		do
		{
			info = TwitchDeckManager.Instance.Draw();
		} while (info!.Id == "asquared31415.TwitchIntegration.Surprise");

		var eventData = DataManager.Instance.GetDataForEvent(info);
		Log.Info($"Surprise triggering {info}({info.Id})");
		info.Trigger(eventData);
	}
}
