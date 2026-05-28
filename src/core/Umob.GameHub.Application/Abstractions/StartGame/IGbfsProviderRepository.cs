using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.StartGame
{
	public interface IGbfsProviderRepository
    {
        Task<GbfsProvider?> GetByIdAsync(
            long providerId,
            CancellationToken cancellationToken = default);
    }
}
