using STRINGS;

namespace ONITwitchCore.Commands;

public class BansheeWailCommand : CommandBase
{
	public override bool Condition(object data)
	{
		return Components.LiveMinionIdentities.Count > 0;
	}

	public override void Run(object data)
	{
		var choreType = Db.Get().ChoreTypes.BansheeWail;
		foreach (var minionIdentity in Components.LiveMinionIdentities.Items)
		{
			var provider = minionIdentity.GetComponent<ChoreProvider>();
			var notification = new Notification(
				DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_NAME,
				NotificationType.Bad,
				(notificationList, _) =>
					DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_TOOLTIP +
					notificationList.ReduceMessages(false)
			);

			var chore = new BansheeChore(choreType, provider, notification);
			provider.AddChore(chore);
		}
	}
}
