using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KSerialization;
using ONITwitch.Settings.Components;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitch.Voting;

[SerializationConfig(MemberSerialization.OptIn)]
internal class VoteFile : KMonoBehaviour
{
	private const float FileUpdateTime = 1f / 3f;
	private float accum;

	public void Update()
	{
		accum += Time.unscaledDeltaTime;
		while (accum >= FileUpdateTime)
		{
			accum -= FileUpdateTime;

			if (VoteController.Instance != null)
			{
				var voteController = VoteController.Instance;
				string fileText;
				switch (voteController.State)
				{
					case VoteController.VotingState.NotStarted:
					{
						fileText = STRINGS.ONITWITCH.VOTE_INFO_FILE.NOT_STARTED;
						break;
					}
					case VoteController.VotingState.VoteInProgress:
					{
						var sb = new StringBuilder();
						// In case something gets desynchronized and the vote is null, just don't display anything
						var votes = voteController.CurrentVote?.Votes ?? new List<Vote.VoteCount>().AsReadOnly();
						for (var idx = 0; idx < votes.Count; idx++)
						{
							sb.Append($"{idx + 1}: {votes[idx].EventInfo} ({votes[idx].Count})\n");
						}

						fileText = string.Format(
							STRINGS.ONITWITCH.VOTE_INFO_FILE.IN_PROGRESS_FORMAT,
							voteController.VoteTimeRemaining,
							sb
						);
						break;
					}
					case VoteController.VotingState.VoteDelay:
					{
						fileText = string.Format(
							STRINGS.ONITWITCH.VOTE_INFO_FILE.VOTE_OVER_FORMAT,
							voteController.VoteDelayRemaining
						);
						break;
					}
					case VoteController.VotingState.Error:
					{
						fileText = STRINGS.ONITWITCH.VOTE_INFO_FILE.ERROR;
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}

				var filePath = Path.Combine(TwitchModInfo.MainModFolder, TwitchSettings.SettingsData.VotesPath);
				Task.Run(
					() => { File.WriteAllText(filePath, fileText); }
				);
			}
		}
	}

	protected override void OnCleanUp()
	{
		var filePath = Path.Combine(TwitchModInfo.MainModFolder, TwitchSettings.SettingsData.VotesPath);
		File.WriteAllText(filePath, "Voting not yet started");
		base.OnCleanUp();
	}
}
