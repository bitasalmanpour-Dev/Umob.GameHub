using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.StartGame
{
	public interface IProviderFeedRepository
	{
		Task<ProviderFeed?> GetByIdAsync(
			long providerFeedsId,
			CancellationToken cancellationToken = default);
	}
}
