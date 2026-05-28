using Refit;
using Umob.GameHub.Application.Abstractions;

namespace Umob.GameHub.Infrastructure.Services
{
	public sealed class GbfsCaller : IGbfsCaller
    {
        public async Task<string> GetJsonAsync(
            string url,
            CancellationToken cancellationToken = default)
        {
            var api = RestService.For<IGbfsApiClient>(url);

            using var response = await api.GetAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
