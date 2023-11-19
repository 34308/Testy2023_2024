using DandDu.Controllers;
using JJ_API.Models.DTO;
using JJ_API.Models.DTO;
using JJ_API.Service;
using JJ_API.Service.Buisneess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JJ_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouteSpotController
    {
        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<UserController> _logger;

        public RouteSpotController(ILogger<UserController> logger)
        {
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            _logger = logger;
        }
      
        [HttpPost("ChangeRouteSpotsOrder")]
        public string GetAllRoutesForUser([FromBody] List<RouteSpot> routeSpots)
        {
            return JsonConvert.SerializeObject(RouteSpotService.ChangeRouteSpotOrder(routeSpots, SqlServerSettings.ConnectionString));
        }
    }
}
