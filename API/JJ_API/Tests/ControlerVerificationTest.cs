using DandDu.Controllers;
using JJ_API.Controllers;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service;
using JJ_API.Service.Buisneess;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;

namespace JJ_API.Tests
{
    [TestClass]
    public class ControlerVerificationTest
    {
        private string _connectionString = "Data Source=\"localhost\\sqljj\";Initial Catalog=JJDBTests;User=sa;Password = 5540;Persist Security Info=True;Pooling=False;TrustServerCertificate=true";
        private int _userId = 0;
        private int _touristSpotId = 0;
        string _token = "";
        static readonly HttpClient client = new HttpClient();

        private ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        UserController _userController;
        CommentController _commentController;
        private TestContext _testContextInstance;
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
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

            this._userController = new UserController(logger, true);

            _token = ((ResultSignInDto)JsonConvert.DeserializeObject<ReadResponseClas>(_userController.SignIn(signInDto)).Data).Token;

        }
        [TestCleanup]
        public void Clean()
        {
            UserService.RemoveUser(_userId, _connectionString);
            TouristSpotService.RemoveTouristSpots(new List<int>() { _touristSpotId }, _connectionString);
        }
        [TestMethod]
        public async Task AddCommentTestVerificationAsync()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;
            try
            {
                string json = JsonConvert.SerializeObject(commentDto);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._token);

                HttpResponseMessage response = await client.PostAsync("https://localhost:7225/Comment/add/" + this._userId, data);
                string responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<Models.ApiResult<Service.Buisneess.Results, int>>(responseContent);
                Assert.IsNotNull(result);
                Assert.AreEqual(0, (int)result.Status);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }
    }
}
