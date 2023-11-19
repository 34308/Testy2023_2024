using JJ_API.Models.DAO;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace JJ_API.Models.DTO
{
    public class RoutePin
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "region")]
        public CoordinatesDto Coordinates { get; set; }


        public RoutePin( RoutePinDao routePinDao) {
            Id = routePinDao.Id;
            Name = routePinDao.Name;
            Coordinates=new CoordinatesDto(routePinDao.Latitude,routePinDao.Longitude);
        }
    }

}
