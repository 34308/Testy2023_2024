using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using JJ_API.Service;
using JJ_API.Service.Buisneess;
using JJ_API.Models;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DTO;
using JJ_API.Configuration;
using JJ_API.Interfaces;

namespace DandDu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        SqlServerSettings SqlServerSettings=new SqlServerSettings();
        JwtSettings JwtSettings=new JwtSettings();
        private readonly ILogger<UserController> _logger;
        private readonly INotificationService _notificationService;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            var properties= PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            JwtSettings = propertiesSingleton.jwtSettings;
        }
        public UserController(ILogger<UserController> logger, INotificationService notiificationService, bool testDatabase=false)
        {
            _logger = logger;
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            if (testDatabase)
            {
                this.SqlServerSettings.DataBase = "JJDBTests";
            }
            JwtSettings = propertiesSingleton.jwtSettings;
            _notificationService = notiificationService;
        }
        [HttpPost("SignUp")]
        public string SignUp([FromBody] RegisterDto input)
        {
            var results = RegistrationService.RegisterUser(input, SqlServerSettings.ConnectionString);
            return JsonConvert.SerializeObject(results);
        }
        [HttpPost("SignIn")]
        public string SignIn([FromBody] SignInDto input)
        {

            return JsonConvert.SerializeObject(SignInService.SignIn(input, SqlServerSettings.ConnectionString,JwtSettings.SecretKey,JwtSettings.Issuer,JwtSettings.Audience));
        }
        [Authorize]
        [HttpGet("CheckStatus")]
        public string CheckLogin()
        {

            return JsonConvert.SerializeObject(true);
        }
        [HttpGet("GetAvatarForUser/{id}")]
        public string GetAvatar([FromRoute]int id)
        {

            return JsonConvert.SerializeObject(AvatarService.GetAvatarForUser(id,SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpGet("islogged")]
        public string CheckIfLogged()
        {
            return JsonConvert.SerializeObject(new ApiResult<string,object>("0","OK",null));
        }
        [Authorize]
        [AuthorizeUserId]

        [HttpPost("update/{UserId}")]
        public string ChangeDetails([FromRoute] int UserId, [FromBody] RegisterDto register)
        {
            return JsonConvert.SerializeObject(UserService.UpdateUser(UserId,register,SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("updatePassword/{UserId}")]
        public string ChangePassword([FromRoute] int UserId, [FromBody] ChangePasswordDto changePassword)
        {
            return JsonConvert.SerializeObject(UserService.ChangeUserPassword(UserId, changePassword, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("updateLogin/{UserId}")]
        public string ChangeLogin([FromRoute] int UserId, [FromBody] ChangeLoginDto changeLogin)
        {
            return JsonConvert.SerializeObject(UserService.UpdateLogin(UserId, changeLogin, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("updateEmail/{UserId}")]
        public string ChangeEmail([FromRoute] int UserId, [FromBody] ChangeEmailDto changeEmailDto)
        {
            return JsonConvert.SerializeObject(UserService.UpdateEmail(UserId, changeEmailDto, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("setNewAvatar/{userId}")]
        public string SetNewAvatar([FromRoute] int userId, [FromBody] string avatar)
        {
            return JsonConvert.SerializeObject(UserService.ChangeAvatarForNewOne(userId, avatar, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [HttpPost("updateAvatar/{userId}/{avatarId}")]
        public string UpdateAvatar([FromRoute] int userId, [FromRoute] int avatarId)
        {
            return JsonConvert.SerializeObject(UserService.ChangeAvatarForExistingOne(userId, avatarId, SqlServerSettings.ConnectionString));
        }

        [HttpGet("getAllBasicAvatars")]
        public string GetAllAvatars()
        {
            return JsonConvert.SerializeObject(AvatarService.GetAllBasicAvatars(SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpGet("getUserNotifications/{UserId}")]
        public string GetUserNotifications([FromRoute]int userId)
        {
            return JsonConvert.SerializeObject(_notificationService.GetNotificationForUser(userId, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpGet("setUserNotification/{UserId}/{Nid}")]
        public string SetNotificatioToChecked([FromRoute] int UserId,int Nid)
        {
            return JsonConvert.SerializeObject(_notificationService.SetNotificatioToChecked(UserId, Nid,SqlServerSettings.ConnectionString));
        }
        
    }
}