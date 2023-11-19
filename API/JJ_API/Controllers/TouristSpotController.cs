using DandDu.Controllers;
using JJ_API.Service.Buisneess;
using JJ_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DTO;

namespace JJ_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TouristSpotController : Controller
    {
        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<UserController> _logger;

        public TouristSpotController(ILogger<UserController> logger)
        {
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            _logger = logger;
        }
        [HttpGet("TouristSpotsBasic")]
        public string GetTouristSpotsBasic()
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.GetAllBasicTouristSpots(SqlServerSettings.ConnectionString));
        }
        [HttpGet("TouristSpots")]
        public string GetTouristSpots()
        {
            return JsonConvert.SerializeObject(TouristSpotService.GetAllTouristSpots(SqlServerSettings.ConnectionString));
        }
        [HttpPost("TouristSpotsForCityBasic/{city}")]
        public string GetTouristSpotsForCity([FromRoute] string city)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.GetBasicTouristSpotsForCity(city, SqlServerSettings.ConnectionString));
        }
        [HttpPost("TouristSpotsForCity/{city}")]
        public string GetTouristSpots([FromRoute] string city)
        {
            return JsonConvert.SerializeObject(TouristSpotService.GetAllTouristSpotsForCity(city, SqlServerSettings.ConnectionString));
        }
        [HttpPost("TouristSpotsForCity/{city}/{id}/{allPhotos}")]
        public string GetTouristSpots([FromRoute] string city, [FromRoute] int id, bool allPhotos)
        {
            return JsonConvert.SerializeObject(TouristSpotService.GetTouristSpot(id, SqlServerSettings.ConnectionString, allPhotos));
        }
        [Authorize]
        [HttpGet("VisitedSpots/{id}")]
        public string GetVisitedSpots([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.VisitedTouristSpotsForUser(id, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpGet("AddVisitedSpot/{SpotId}/{UserId}")]
        public string AddVisitedSpot([FromRoute] int SpotId, int UserId)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.AddVisitedTouristSpotForUser(UserId, SpotId, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpGet("RemoveVisitedSpot/{SpotId}/{UserId}")]
        public string RemoveVisitedSpot([FromRoute] int SpotId, int UserId)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.RemoveVisitedTouristSpotForUser(UserId, SpotId, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpGet("getVisitedSpotsId/{UserId}")]
        public string GetVisitedSpotsId(int UserId)
        {
            return JsonConvert.SerializeObject(TouristSpotService.GetVisitedSpots(UserId, SqlServerSettings.ConnectionString));
        }
        [HttpPost("getSpotsForPins")]
        public string GetSpotsForPins([FromBody] PinsIds ids)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.GetTouristSpotsForMapPins(ids, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpGet("getVisitedSpotsForUser/{UserId}")]
        public string GetVisitedSpotsForUser([FromRoute] int UserId)
        {
            return JsonConvert.SerializeObject(BasicTouristSpotService.GetAllVisitedTouristSpotsForUser(UserId, SqlServerSettings.ConnectionString));
        }
       
    }
}
