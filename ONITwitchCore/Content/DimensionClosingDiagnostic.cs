using ONITwitchLib;
using ONITwitchLib.Logger;

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
