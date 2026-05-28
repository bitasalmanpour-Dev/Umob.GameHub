using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;

namespace Umob.GameHub.Infrastructure.Repositories
{
	public class GameAttemptRepository(ApplicationDbContext dbContext)
		: IGameAttemptRepository
	{
		public async Task AddAsync(
			GameAttempt gameAttempt,
			CancellationToken cancellationToken = default)
		{
			dbContext.GameAttempts.Add(gameAttempt);

			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<GameAttempt?> GetForAnswerAsync(
		   long attemptId,
		   long userId,
		   long questionId,
		   CancellationToken cancellationToken = default)
		{
			return await dbContext.GameAttempts
				.Include(attempt => attempt.Questions
					.Where(question => question.Id == questionId))
				.ThenInclude(question => question.Options)
				.FirstOrDefaultAsync(
					attempt => attempt.Id == attemptId &&
							   attempt.UserId == userId,
					cancellationToken);
		}

		public async Task<IReadOnlyList<GameAttempt>> GetByUserIdAsync(
		    long userId,
		    CancellationToken cancellationToken = default)
		{
			//why: we just read, AsNoTracking help to improve performance.
			return await dbContext.GameAttempts
				.AsNoTracking()
				.Where(x => x.UserId == userId)
				.OrderByDescending(x => x.StartedOn)
				.ToListAsync(cancellationToken);
		}

		public async Task SaveChangesAsync(
			CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
