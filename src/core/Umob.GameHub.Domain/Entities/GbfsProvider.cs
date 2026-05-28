namespace Umob.GameHub.Domain.Entities
{
	public sealed class GbfsProvider
	{
		public long Id { get; private set; }

		public string Name { get; private set; } = string.Empty;

		public string City { get; private set; } = string.Empty;

		public string CountryCode { get; private set; } = string.Empty;

		public string GbfsAutoDiscoveryUrl { get; private set; } = string.Empty;

		public string SystemId { get; private set; } = string.Empty;

		public bool IsActive { get; private set; }

		public DateTime CreatedOn { get; private set; }

        public GbfsProvider()
        {
            
        }
    }
}
