using Newtonsoft.Json;

namespace JJ_API.Models.DTO
{
    public class CoordinatesDto
    {
        [JsonProperty(PropertyName = "latitude")]

        public float Latitude { get; set; }
        [JsonProperty(PropertyName = "longitude")]

        public float Longitude { get; set; }
        [JsonProperty(PropertyName = "latitudeDelta")]

        public float LatitudeDelta { get; set; } = (float)0.01;
        [JsonProperty(PropertyName = "longitudeDelta")]

        public float LongitudeDelta { get; set; } = (float)0.01;


        public CoordinatesDto(float latitude, float longitude, float longitudeDelta, float latitudeDelta)
        {
            Latitude = latitude;
            Longitude = longitude;
            LongitudeDelta = longitudeDelta;
            LatitudeDelta = latitudeDelta;
        }
        public CoordinatesDto(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

        }
        public CoordinatesDto()
        {

        }
    }

}
