using ONITwitchLib;
using ONITwitchLib.Logger;

namespace ONITwitchCore.Content;

public class DimensionClosingDiagnostic : ColonyDiagnostic
{
	public const string Id = TwitchModInfo.ModPrefix + "PocketDimIsClosingSoon";

	public DimensionClosingDiagnostic(int worldID) : base(
		worldID,
		STRINGS.ONITWITCH.DIAGNOSTICS.DIMENSION_CLOSING.DIAGNOSTIC_NAME
	)
	{
		AddCriterion(
			Id,
			new DiagnosticCriterion(STRINGS.ONITWITCH.DIAGNOSTICS.DIMENSION_CLOSING.CRITERION_NAME, EvaluateDiagnostic)
		);
	}

	private static readonly DiagnosticResult ErrorDiagnostic = new(
		DiagnosticResult.Opinion.DuplicantThreatening,
		STRINGS.ONITWITCH.DIAGNOSTICS.DIMENSION_CLOSING.ERROR
	);

	private static readonly DiagnosticResult NormalDiagnostic = new(
		DiagnosticResult.Opinion.Normal,
		STRINGS.ONITWITCH.DIAGNOSTICS.DIMENSION_CLOSING.NORMAL
	);

	private static readonly DiagnosticResult LowTimeDiagnostic = new(
		DiagnosticResult.Opinion.Bad,
		STRINGS.ONITWITCH.DIAGNOSTICS.DIMENSION_CLOSING.CLOSING
	);

	private DiagnosticResult EvaluateDiagnostic()
	{
		var world = ClusterManager.Instance.GetWorld(worldID);
		if (world == null)
		{
			Log.Debug($"Pocket dimension closing diagnostic on null world idx {worldID}");
			return ErrorDiagnostic;
		}

		if (!world.TryGetComponent(out Cmps.PocketDimension.PocketDimension pocketDimension))
		{
			Log.Warn($"Pocket dimension closing diagnostic on incorrect world {world} (id {world.id})");
			return ErrorDiagnostic;
		}

		// warn at 0.5 cycles remaining
		return pocketDimension.Lifetime <= 0.5 * Constants.SECONDS_PER_CYCLE ? LowTimeDiagnostic : NormalDiagnostic;
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}
}
