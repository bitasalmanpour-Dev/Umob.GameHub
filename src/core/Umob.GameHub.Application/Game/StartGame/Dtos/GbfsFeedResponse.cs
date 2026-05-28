using System.Text.Json.Serialization;

namespace Umob.GameHub.Application.Game.StartGame.Dtos
{
	public class GbfsFeedResponse
    {
        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }

        [JsonPropertyName("data")]
        public GbfsDiscoveryData? Data { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }
    }

    public class GbfsDiscoveryData
    {
        [JsonPropertyName("feeds")]
        public List<GbfsFeed> Feeds { get; set; } = new();
    }

    public class GbfsFeed
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
