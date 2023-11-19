using System.Text.Json.Serialization;

namespace JJ_API.Models.DTO
{
    public class GetTouristSpotDto
    {
        [JsonPropertyName("name")]
        public string CityName { get; set;}
    }
}
