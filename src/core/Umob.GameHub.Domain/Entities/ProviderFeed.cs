namespace Umob.GameHub.Domain.Entities
{
	public sealed class ProviderFeed
	{
		public long Id { get; private set; }

		public string FeedName { get; private set; } = string.Empty;

		public string Url { get; private set; } = string.Empty;

		public long ProviderId { get; private set; }

        public ProviderFeed()
        {
            
        }
    }
}
