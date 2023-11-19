using DandDu.Controllers;
using JJ_API.Service.Buisneess;
using JJ_API.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JJ_API.Configuration;

namespace JJ_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : Controller
    {

        JwtSettings jwt = new JwtSettings();

        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<UserController> _logger;

        public CityController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;

        }

        [HttpGet("AllCitys")]
        public string GetTouristSpots()
        {
            return JsonConvert.SerializeObject(CityService.GetAllCitys(SqlServerSettings.ConnectionString));
        }
    }

}
