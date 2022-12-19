using EventLib;

namespace ONITwitchCore.Commands;

public class SurpriseCommand : CommandBase
{
	public override void Run(object data)
	{
		EventInfo info;
		do
		{
			info = TwitchDeckManager.Instance.Draw();
		} while ((info != null) && (info.Id == DefaultCommands.NamespaceId("Surprise")));

		if (info != null)
		{
			var eventData = DataManager.Instance.GetDataForEvent(info);
			Debug.Log($"[Twitch Integration] Surprise triggering {info}({info.Id})");
			EventManager.Instance.TriggerEvent(info, eventData);
		}
	}
}
