using MediatR;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.GameAttemptsReport.Dtos;

namespace Umob.GameHub.Application.Game.GameAttemptsReport
{
	public sealed class GetUserGameAttemptsQueryHandler(IGameAttemptRepository gameAttemptRepository)
		: IRequestHandler<GetUserGameAttemptsQuery, IReadOnlyList<GetUserGameAttemptsResponse>>
	{
		public async Task<IReadOnlyList<GetUserGameAttemptsResponse>> Handle(
			GetUserGameAttemptsQuery request,
			CancellationToken cancellationToken)
		{
			var attempts = await gameAttemptRepository.GetByUserIdAsync(
				request.UserId,
				cancellationToken);

			return attempts
				.Select(attempt => new GetUserGameAttemptsResponse(
					attempt.Id,
					attempt.Score,
					attempt.IsWon,
					attempt.StartedOn,
					attempt.EndedOn,
					attempt.DurationSeconds))
				.ToList();
		}
	}
}
