using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KSerialization;
using ONITwitchCore.Config;
using ONITwitchCore.Settings;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore;

[SerializationConfig(MemberSerialization.OptIn)]
public class VoteFile : KMonoBehaviour
{
	private const float FileUpdateTime = 1f / 3f;
	private float accum;

	public void Update()
	{
		accum += Time.unscaledDeltaTime;
		while (accum >= FileUpdateTime)
		{
			accum -= FileUpdateTime;

			if (Game.Instance.TryGetComponent<VoteController>(out var voteController))
			{
				string fileText;
				switch (voteController.State)
				{
					case VoteController.VotingState.NotStarted:
					{
						fileText = "Voting not yet started";
						break;
					}
					case VoteController.VotingState.VoteInProgress:
					{
						var sb = new StringBuilder();
						sb.Append(
							$"Current Vote ({Mathf.RoundToInt(voteController.VoteTimeRemaining)}s)\n"
						);
						var votes = voteController.CurrentVote.Votes;
						for (var idx = 0; idx < votes.Count; idx++)
						{
							sb.Append($"{idx + 1}: {votes[idx].EventInfo} ({votes[idx].Count})\n");
						}

						fileText = sb.ToString();
						break;
					}
					case VoteController.VotingState.VoteDelay:
					{
						fileText = $"Vote Over ({voteController.VoteDelayRemaining:F0} seconds to next vote)";
						break;
					}
					case VoteController.VotingState.Error:
					{
						fileText = "An error occurred in an event";
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}

				var filePath = Path.Combine(TwitchModInfo.MainModFolder, GenericModSettings.Data.VotesPath);
				Task.Run(
					() => { File.WriteAllText(filePath, fileText); }
				);
			}
		}
	}

	protected override void OnCleanUp()
	{
		var filePath = Path.Combine(TwitchModInfo.MainModFolder, GenericModSettings.Data.VotesPath);
		File.WriteAllText(filePath, "Voting not yet started");
		base.OnCleanUp();
	}
}
