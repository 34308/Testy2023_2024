using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using JJ_API.Service;
using JJ_API.Service.Buisneess;
using JJ_API.Models.DTO;

namespace DandDu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouteController : ControllerBase
    {
        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<UserController> _logger;

        public RouteController(ILogger<UserController> logger)
        {         
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("getUserRoutes/{id}")]
        public string GetAllRoutesForUser([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(RouteService.GetAllRoutesForUser(id, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("AddUserRoute")]
        public string AddRouteForUser([FromBody] RouteDTO route)
        {
            return JsonConvert.SerializeObject(RouteService.AddRoutesForUser(route, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpGet("RemoveUserRoute/{id}")]
        public string RemoveRouteForUser([FromRoute] int id)
        {

            return JsonConvert.SerializeObject(RouteService.RemoveRoutesForUser(id, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("UpdateRouteName/")]
        public string UpdateRouteName([FromBody] RouteDTO routeDTO)
        {

            return JsonConvert.SerializeObject(RouteService.ChangeNameForRoute(routeDTO, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("AddSpotToRoute")]
        public string AddSpotToRoute([FromBody] RouteSpot routeSpot)
        {
            return JsonConvert.SerializeObject(RouteSpotService.AddRouteSpotToRoute(routeSpot, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("RemoveSpotFromRoute")]
        public string RemoveSpotFromRoute([FromBody] RouteSpot routeSpot)
        {
            return JsonConvert.SerializeObject(RouteSpotService.RemoveSpotFromRoute(routeSpot, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("GetRoutePinsForRoute/{calculatePatch}")]
        public string GetRoytePinsForRoute([FromBody] UserLocWithRoute route,[FromRoute] bool calculatePath)
        {
            return JsonConvert.SerializeObject(RouteService.GetMapRouteForChoosenRoute(route,calculatePath, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpGet("GetSpotsForRoute/{routeId}/{userId}")]
        public string GetSpotsForRoute([FromRoute] int routeId, int userId)
        {
            return JsonConvert.SerializeObject(RouteService.GetSpotsForRouteForUser(routeId, userId, SqlServerSettings.ConnectionString));
        }
    }
}