using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;

namespace Umob.GameHub.Infrastructure.Repositories
{
	public class QuestionTemplateRepository(ApplicationDbContext dbContext)
		: IQuestionTemplateRepository
	{
		public async Task<IReadOnlyList<QuestionTemplate>> GetActiveTemplatesWithFieldsAsync(
		CancellationToken cancellationToken = default)
		{
			return await dbContext.QuestionTemplates
			   .AsNoTracking()
			   .Include(x => x.GbfsFields)
			   .ThenInclude(x => x.ProviderFeed)
			   .Where(x => x.IsActive)
			   .OrderBy(x => x.Id)
			   .ToListAsync(cancellationToken);
		}
	}
}
