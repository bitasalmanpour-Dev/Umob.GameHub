using System.Text.Json.Serialization;

namespace Umob.GameHub.Application.Game.StartGame.Dtos
{
	public class GbfsStationStatusResponse
    {
        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }

        [JsonPropertyName("data")]
        public GbfsStationStatusData Data { get; set; } = new();

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }

    public class GbfsStationStatusData
    {
        [JsonPropertyName("stations")]
        public List<GbfsStationStatus> Stations { get; set; } = new();
    }

    public class GbfsStationStatus
    {
        [JsonPropertyName("station_id")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("num_vehicles_available")]
        public int NumVehiclesAvailable { get; set; }

        [JsonPropertyName("num_vehicles_disabled")]
        public int NumVehiclesDisabled { get; set; }

        [JsonPropertyName("num_docks_available")]
        public int NumDocksAvailable { get; set; }

        [JsonPropertyName("num_docks_disabled")]
        public int NumDocksDisabled { get; set; }

        [JsonPropertyName("last_reported")]
        public DateTime LastReported { get; set; }

        [JsonPropertyName("is_installed")]
        public bool IsInstalled { get; set; }

        [JsonPropertyName("is_renting")]
        public bool IsRenting { get; set; }

        [JsonPropertyName("is_returning")]
        public bool IsReturning { get; set; }

        [JsonPropertyName("vehicle_docks_available")]
        public List<VehicleDockAvailable> VehicleDocksAvailable { get; set; } = new();

        [JsonPropertyName("vehicle_types_available")]
        public List<VehicleTypeAvailable> VehicleTypesAvailable { get; set; } = new();
    }

    public class VehicleDockAvailable
    {
        [JsonPropertyName("vehicle_type_ids")]
        public List<string> VehicleTypeIds { get; set; } = new();

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class VehicleTypeAvailable
    {
        [JsonPropertyName("vehicle_type_id")]
        public string VehicleTypeId { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
