using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.StartGame
{
	public interface IGameAttemptRepository
    {
        Task AddAsync(
           GameAttempt gameAttempt,
           CancellationToken cancellationToken = default);
		Task<GameAttempt?> GetForAnswerAsync(
		   long attemptId,
		   long userId,
		   long questionId,
		   CancellationToken cancellationToken = default);

		Task<IReadOnlyList<GameAttempt>> GetByUserIdAsync(
		   long userId,
		   CancellationToken cancellationToken = default);

		Task SaveChangesAsync(
			CancellationToken cancellationToken = default);
	}
}
