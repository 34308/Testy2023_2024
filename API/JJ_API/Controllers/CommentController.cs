using DandDu.Controllers;
using JJ_API.Service.Buisneess;
using JJ_API.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DTO;
using JJ_API.Interfaces;


namespace JJ_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentServiceWrapper _commentServiceWrapper;

        SqlServerSettings SqlServerSettings = new SqlServerSettings();

        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger, ICommentServiceWrapper commentServiceWrapper, bool isTest=false)
        {
            _logger = logger;
            var properties = PropertiesSingletonBase.Load();
            PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
            SqlServerSettings = propertiesSingleton.FactoryLinkDBConnection;
            this._commentServiceWrapper = commentServiceWrapper;
            if (isTest)
            {
                SqlServerSettings.DataBase = "JJDBTests";
            }
        }

        [HttpGet("AllComentsForSpot/{id}")]
        public string GetComments([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(_commentServiceWrapper.GetCommentsForTouristSpot(id, SqlServerSettings.ConnectionString));
        }
        [HttpGet("AllComentsForParent/{id}")]
        public string GetCommentsForParent([FromRoute] int id)
        {
            return JsonConvert.SerializeObject(_commentServiceWrapper.GetCommentsForParent(id, SqlServerSettings.ConnectionString));
        }
        [HttpGet("AllComentsForUser/{userId}")]
        public string GetCommentsForUser([FromRoute] int userId)
        {
            return JsonConvert.SerializeObject(_commentServiceWrapper.GetAllCommentsForUser(userId, SqlServerSettings.ConnectionString));
        }

        [Authorize]
        [AuthorizeUserId]
        [HttpGet("remove/{userId}/{id}")]
        public IActionResult RemoveComment([FromRoute] int userId, [FromRoute] int id)
        {
            return Ok(JsonConvert.SerializeObject(_commentServiceWrapper.RemoveComment(userId, new List<int> { id }, SqlServerSettings.ConnectionString)));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("add/{userId}")]
        public string AddComment([FromBody] CommentDto comment, int userId)
        {

            return JsonConvert.SerializeObject(_commentServiceWrapper.AddComment(comment, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("comment/{userId}")]
        public string AddCommentForComment([FromBody] CommentForCommentDto comment, int userId)
        {
            return JsonConvert.SerializeObject(_commentServiceWrapper.AddCommentForComment(comment, SqlServerSettings.ConnectionString));
        }
        [Authorize]
        [AuthorizeUserId]
        [HttpPost("update/{userId}")]
        public string UpdateComment([FromBody] CommentDto comment, int userId)
        {
            return JsonConvert.SerializeObject(_commentServiceWrapper.EditComment(comment, SqlServerSettings.ConnectionString));
        }
    }
}
