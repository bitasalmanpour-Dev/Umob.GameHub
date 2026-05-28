using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;

namespace Umob.GameHub.Infrastructure.Repositories
{
	public sealed class GbfsProviderRepository(ApplicationDbContext dbContext) 
		: IGbfsProviderRepository
	{
		public async Task<GbfsProvider?> GetByIdAsync(
			long providerId,
			CancellationToken cancellationToken = default) => await dbContext.GbfsProviders
				.AsNoTracking()
				.FirstOrDefaultAsync(
					x => x.Id == providerId && x.IsActive,
					cancellationToken);
	}
}
