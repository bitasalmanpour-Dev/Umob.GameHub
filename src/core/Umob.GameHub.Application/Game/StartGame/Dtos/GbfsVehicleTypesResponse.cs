using System.Text.Json.Serialization;

namespace Umob.GameHub.Application.Game.StartGame.Dtos
{
	public class GbfsVehicleTypesResponse
    {
        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }

        [JsonPropertyName("data")]
        public GbfsVehicleTypesData Data { get; set; } = new();

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }

    public class GbfsVehicleTypesData
    {
        [JsonPropertyName("vehicle_types")]
        public List<GbfsVehicleType> VehicleTypes { get; set; } = new();
    }

    public class GbfsVehicleType
    {
        [JsonPropertyName("vehicle_type_id")]
        public string VehicleTypeId { get; set; } = string.Empty;

        [JsonPropertyName("form_factor")]
        public string FormFactor { get; set; } = string.Empty;

        [JsonPropertyName("propulsion_type")]
        public string PropulsionType { get; set; } = string.Empty;

        [JsonPropertyName("max_range_meters")]
        public double MaxRangeMeters { get; set; }

        [JsonPropertyName("name")]
        public List<GbfsVehicleTypeName> Name { get; set; } = new();

        [JsonPropertyName("default_pricing_plan_id")]
        public string DefaultPricingPlanId { get; set; } = string.Empty;

        [JsonPropertyName("pricing_plan_ids")]
        public List<string> PricingPlanIds { get; set; } = new();
    }

    public class GbfsVehicleTypeName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
    }
}
