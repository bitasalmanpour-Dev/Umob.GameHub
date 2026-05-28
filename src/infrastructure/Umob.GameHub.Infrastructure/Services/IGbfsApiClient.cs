using Refit;

namespace Umob.GameHub.Infrastructure.Services;

public interface IGbfsApiClient
{
	[Get("")]
	Task<HttpResponseMessage> GetAsync(
		CancellationToken cancellationToken = default);
}
