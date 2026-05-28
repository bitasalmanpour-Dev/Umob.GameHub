namespace Umob.GameHub.Application.Abstractions
{
	public interface IGbfsCaller
    {
        Task<string> GetJsonAsync(
            string url,
            CancellationToken cancellationToken = default);
    }
}
