using ONITwitchCore.Cmps.PocketDimension;
using ONITwitchLib;

namespace ONITwitchCore.Content;

public class DimensionClosingDiagnostic : ColonyDiagnostic
{
	public const string Id = TwitchModInfo.ModPrefix + "PocketDimIsClosingSoon";

	public DimensionClosingDiagnostic(int worldID) : base(worldID, "Time Remaining")
	{
		AddCriterion(Id, new DiagnosticCriterion("Pocket Dimension closing soon", EvaluateDiagnostic));
	}

	private static readonly DiagnosticResult ErrorDiagnostic = new(
		DiagnosticResult.Opinion.DuplicantThreatening,
		"Error"
	);

	private static readonly DiagnosticResult NormalDiagnostic = new(
		DiagnosticResult.Opinion.Normal,
		"Dimension Normal"
	);

	private static readonly DiagnosticResult LowTimeDiagnostic = new(
		DiagnosticResult.Opinion.Bad,
		"Low Time Remaining"
	);

	// Warn at 0.5 cycles remaining
	public const float RemainingFractionThreshold = 0.5f / PocketDimension.MaxCyclesLifetime;

	private DiagnosticResult EvaluateDiagnostic()
	{
		var world = ClusterManager.Instance.GetWorld(worldID);
		if (world == null)
		{
			Debug.LogWarning($"[Twitch Integration] Pocket dimension closing diagnostic on null world idx {worldID}");
			return ErrorDiagnostic;
		}

		if (!world.TryGetComponent(out PocketDimension pocketDimension))
		{
			Debug.LogWarning(
				$"[Twitch Integration] Pocket dimension closing diagnostic on incorrect world {world} (id {world.id})"
			);
			return ErrorDiagnostic;
		}

		var remaining = pocketDimension.GetFractionLifetimeRemaining();

		return remaining <= RemainingFractionThreshold ? LowTimeDiagnostic : NormalDiagnostic;
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
