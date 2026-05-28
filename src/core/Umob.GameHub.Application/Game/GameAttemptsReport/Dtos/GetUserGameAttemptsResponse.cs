namespace Umob.GameHub.Application.Game.GameAttemptsReport.Dtos
{
	public sealed record GetUserGameAttemptsResponse(
		long AttemptId,
		int Score,
		bool? IsWon,
		DateTime StartedOn,
		DateTime? EndedOn,
		int DurationSeconds);
}
