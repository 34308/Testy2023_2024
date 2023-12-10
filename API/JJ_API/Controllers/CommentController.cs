using DandDu.Controllers;
using JJ_API.Service.Buisneess;
using JJ_API.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DTO;

namespace JJ_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : Controller
    {

        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<UserController> _logger;

        public CommentController(ILogger<UserController> logger, bool isTest=false)
        {
            _logger = logger;
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            if (isTest)
            {
                SqlServerSettings.DataBase = "JJDBTests";
            }
        }

        [HttpGet("AllComentsForSpot/{id}")]
        public string GetComments([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(CommentService.GetCommentsForTouristSpot(id, SqlServerSettings.ConnectionString));
        }
        [HttpGet("AllComentsForParent/{id}")]
        public string GetCommentsForParent([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(CommentService.GetCommentsForParent(id, SqlServerSettings.ConnectionString));
        }
        [HttpGet("AllComentsForUser/{userId}")]
        public string GetCommentsForUser([FromRoute] int userId)
        {
            return JsonConvert.SerializeObject(CommentService.GetAllCommentsForUser(userId, SqlServerSettings.ConnectionString));
        }

        [Authorize]
        [AuthorizeUserId]
        [HttpGet("remove/{userId}/{id}")]
        public IActionResult RemoveComment([FromRoute] int userId, [FromRoute] int id)
        {
            return Ok(JsonConvert.SerializeObject(CommentService.RemoveComment(userId, new List<int> { id }, SqlServerSettings.ConnectionString)));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("add/{userId}")]
        public string AddComment([FromBody] CommentDto comment, int userId)
        {

            return JsonConvert.SerializeObject(CommentService.AddComment(comment, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("comment/{userId}")]
        public string AddCommentForComment([FromBody] CommentForCommentDto comment, int userId)
        {
            return JsonConvert.SerializeObject(CommentService.AddCommentForComment(comment, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("update/{userId}")]
        public string UpdateComment([FromBody] CommentDto comment, int userId)
        {
            return JsonConvert.SerializeObject(CommentService.EditComment(comment, SqlServerSettings.ConnectionString));
        }
    }
}
