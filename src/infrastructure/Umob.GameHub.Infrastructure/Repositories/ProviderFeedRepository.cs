using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;

namespace Umob.GameHub.Infrastructure.Repositories
{
	public sealed class ProviderFeedRepository(ApplicationDbContext dbContext)
		: IProviderFeedRepository
	{
		public async Task<ProviderFeed?> GetByIdAsync(
			long providerFeedsId,
			CancellationToken cancellationToken = default)
		{
			return await dbContext.ProviderFeeds
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.Id == providerFeedsId,
					cancellationToken);
		}
	}
}
