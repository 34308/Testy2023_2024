using Azure.Core;
using DandDu.Controllers;
using JJ_API.Controllers;
using JJ_API.Interfaces;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service;
using JJ_API.Service.Buisneess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Results = JJ_API.Service.Buisneess.Results;

namespace JJ_API.Tests
{
    [TestClass]
    public class ControlerVerificationTest
    {
        private string _connectionString = "Data Source=\"localhost\\sqljj\";Initial Catalog=JJDBTests;User=sa;Password = 5540;Persist Security Info=True;Pooling=False;TrustServerCertificate=true";
        private int _userId = 0;
        private int _touristSpotId = 0;
        HttpContext _mockHttpContext;
        string _token = "";
        static readonly HttpClient client = new HttpClient();
        private ILogger<CommentController> _comemntlogger;
        Mock<ICommentServiceWrapper> _mock = new Mock<ICommentServiceWrapper>();
        Mock<INotificationService> _mockNotification = new Mock<INotificationService>();


        private ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        CommentController _commentController;
        private HttpContext MockControllerContext(int userid)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Email, "test@wp.pl"),
        new Claim(JwtRegisteredClaimNames.Sub, "TestLogin"),
        new Claim(JwtRegisteredClaimNames.Jti,""+userid ),
        new Claim("role",""+1),
                    }, "AuthenticationType"));

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(ctx => ctx.User).Returns(user);

            return httpContext.Object;
        }
        [TestInitialize]
        public void Prepare()
        {

            SqlServerSettings sqlServerSettings = new SqlServerSettings();
            sqlServerSettings.DataBase = "JJDBTests";

            RegisterDto registerDto = new RegisterDto();
            registerDto.AvatarId = 1;
            registerDto.Email = "test@wp.pl";
            registerDto.Login = "TestLogin";
            registerDto.Password = "Password123@";

            var responseForAddingUser = RegistrationService.RegisterUser(registerDto, _connectionString);
            this._userId = (int)responseForAddingUser.Data;

            this._comemntlogger = loggerFactory.CreateLogger<CommentController>();



            _mockHttpContext = MockControllerContext(_userId);


            Image image = new Image();
            image.Photo = "www.photo.com";
            Address address = new Address();
            address.Street = "test street";
            address.PostalCode = "32-145";
            address.Number = 1;
            address.City = "Tarnów";
            address.Country = "Poland";


            TouristSpot touristSpot = new TouristSpot("New touristSpot", "12:00", "19:00", 1, image, address, "new descritpion", (float)50.322, (float)51.222, "www.website.pl", "48694999", "article", new List<Image>() { image });
            var result = TouristSpotService.AddTouristSpots(touristSpot, _connectionString);
            _touristSpotId = (int)result.Data;
            SignInDto signInDto = new SignInDto();
            signInDto.Password = "Password123@";
            signInDto.Login = "TestLogin";

            ILogger<UserController> logger = loggerFactory.CreateLogger<UserController>();
        
        }
        [TestCleanup]
        public void Clean()
        {
            UserService.RemoveUser(_userId, _connectionString);
            TouristSpotService.RemoveTouristSpots(new List<int>() { _touristSpotId }, _connectionString);
        }
        [TestMethod]
        public void AddCommentControllerTest()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;

            _mock.Setup(service => service.AddComment(commentDto, _connectionString)).Returns(new ApiResult<Results, object>(Results.OK, 1));
            _commentController = new CommentController(_comemntlogger, _mock.Object, true);
            _commentController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext() { HttpContext = _mockHttpContext };

            var resposnseString = (_commentController.AddComment(commentDto, _userId));
            ReadResponseAddComment responseAddComment = JsonConvert.DeserializeObject<ReadResponseAddComment>(resposnseString);

            Assert.IsNotNull(responseAddComment);
            Assert.AreEqual((int)responseAddComment.Status, 0);

        }
        [TestMethod]
        public void RemoveCommentController()
        {

            _mock.Setup(service => service.RemoveComment(this._userId, new List<int> { 1 }, _connectionString)).Returns(new ApiResult<Results, object>(Results.OK, 1));
            _commentController = new CommentController(_comemntlogger, _mock.Object, true);
            _commentController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext() { HttpContext = _mockHttpContext };

            var resposnseString = _commentController.RemoveComment(this._userId, 1);
            var okResult = resposnseString as OkObjectResult;

            Assert.IsNotNull(okResult);

            string jsonString = okResult.Value as string;
            var yourObject = JsonConvert.DeserializeObject<ReadResponseAddComment>(jsonString);

            Assert.AreEqual((int)yourObject.Status, 0);

        }
        [TestMethod]
        public void EditCommentController()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Diffrent";
            commentDto.Description = "DiifrenttestDescription";
            commentDto.Score = 2;
            commentDto.UserId = this._userId;

            _mock.Setup(service => service.EditComment(commentDto, _connectionString)).Returns(new ApiResult<Results, object>(Results.OK, 1));
            _commentController = new CommentController(_comemntlogger, _mock.Object, true);
            _commentController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext() { HttpContext = _mockHttpContext };
            var resposnseString = _commentController.UpdateComment(commentDto, _userId);
            var yourObject = JsonConvert.DeserializeObject<ReadResponseAddComment>(resposnseString);

            Assert.IsNotNull(yourObject);
            Assert.AreEqual((int)yourObject.Status, 0);

        }
    }
}
