using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;

namespace Umob.GameHub.Infrastructure.Repositories
{
	public sealed class QuestionTemplateGbfsFieldRepository(ApplicationDbContext dbContext)
	: IQuestionTemplateGbfsFieldRepository
	{
		public async Task<QuestionTemplateGbfsField?> GetByQuestionTemplateIdAsync(
			long questionTemplateId,
			CancellationToken cancellationToken = default)
		{
			return await dbContext.QuestionTemplateGbfsFields
			   .AsNoTracking()
			   .Include(x => x.ProviderFeed)
			   .FirstOrDefaultAsync(
				   x => x.QuestionTemplateId == questionTemplateId,
				   cancellationToken);
		}
	}
}
