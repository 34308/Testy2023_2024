using JJ_API.Interfaces;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service.Buisneess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace JJ_API.Tests
{
    [TestClass]
    public class CommentsServiceTest
    {
        private string _connectionString = "Data Source=\"localhost\\sqljj\";Initial Catalog=JJDBTests;User=sa;Password = 5540;Persist Security Info=True;Pooling=False;TrustServerCertificate=true";
        private int _userId = 0;
        private int _userId2 = 0;
        private CommentService _commentService;
        private Mock<ICommentRepository> _mockCommentRepo;
        private Mock<INotificationService> _mockNotificationService;

        private int _touristSpotId = 0;
        private TestContext _testContextInstance;
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }
        [TestInitialize]
        public void Prepare()
        {
            _commentService = new CommentService(new NotificationService());
            _mockNotificationService = new Mock<INotificationService>();

            _mockCommentRepo = new Mock<ICommentRepository>();

            RegisterDto registerDto = new RegisterDto();
            registerDto.AvatarId = 1;
            registerDto.Email = "test@wp.pl";
            registerDto.Login = "TestLogin";
            registerDto.Password = "Password123@";

            RegisterDto registerDto2 = new RegisterDto();
            registerDto2.AvatarId = 2;
            registerDto2.Email = "test23@wp.pl";
            registerDto2.Login = "Test22Login";
            registerDto2.Password = "Password1223@";
            var responseForAddingUser = RegistrationService.RegisterUser(registerDto, _connectionString);
            this._userId = (int)responseForAddingUser.Data;
            var responseForAddingUser2 = RegistrationService.RegisterUser(registerDto2, _connectionString);

            this._userId2 = (int)responseForAddingUser2.Data;

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
        }
        [TestCleanup]
        public void Clean()
        {
            UserService.RemoveUser(_userId, _connectionString);
            UserService.RemoveUser(_userId2, _connectionString);

            TouristSpotService.RemoveTouristSpots(new List<int>() { _touristSpotId }, _connectionString);

        }
        [TestCategory("GetAllCommentsForUser")]
        [TestMethod]

        public void GetAllCommentsForUserJ()
        {

            int id = this._userId;
            _mockCommentRepo.Setup(repo => repo.GetAllCommentsForUser(id, _connectionString)).Returns(new List<Comment>());
            _mockCommentRepo.Setup(repo => repo.CheckIfUserExist(id, _connectionString)).Returns(id);

            _commentService = new CommentService(_mockCommentRepo.Object, _mockNotificationService.Object);


            var result = _commentService.GetAllCommentsForUser(id, _connectionString);


            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("GetAllCommentsForUser")]
        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]

        public void GetAllCommentsForUserPositive(int id)
        {

            var result = _commentService.GetAllCommentsForUser(id, _connectionString);

            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("GetAllCommentsForUser")]
        [TestMethod]
        [DataRow(0)]
        [DataRow(-99)]
        [DataRow(int.MaxValue)]
        public void GetAllCommentsForUserNoUserFound(int id)
        {

            var result = _commentService.GetAllCommentsForUser(id, _connectionString);

            Assert.AreEqual(31, (int)result.Status);
        }
        [TestCategory("GetCommentsForTouristSpot")]
        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void GetCommentsForTouristSpotPositive(int id)
        {

            var result = _commentService.GetCommentsForTouristSpot(id, _connectionString);


            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("GetCommentsForTouristSpot")]
        [TestMethod]
        [DataRow(-99)]
        [DataRow(int.MaxValue)]
        public void GetCommentsForTouristSpotNegative(int id)
        {

            var result = _commentService.GetCommentsForTouristSpot(id, _connectionString);


            Assert.AreEqual(4, (int)result.Status);
        }
        [TestCategory("GetCommentsForParent")]
        [TestMethod]

        public void GetCommentsForParentPositive()
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 5;
            commentDto.UserId = _userId;

            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            var result = _commentService.GetCommentsForParent((int)resultAddComment.Data, _connectionString);

            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("GetCommentsForParent")]
        [TestMethod]
        [DataRow(-99)]
        [DataRow(42)]
        [DataRow(int.MaxValue)]
        public void GetCommentsForParentNegative(int id)
        {

            var result = _commentService.GetCommentsForParent(id, _connectionString);

            Assert.AreEqual(33, (int)result.Status);
        }
        [TestCategory("AddCommentTest")]
        [TestMethod]
        public void AddGoodCommentTest()
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 5;
            commentDto.UserId = _userId;

            var result = _commentService.AddComment(commentDto, _connectionString);

            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("AddCommentTest")]
        [TestMethod]
        public void EmptyTitleAddCommentTest()
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = 4;

            var result = _commentService.AddComment(commentDto, _connectionString);


            Assert.AreEqual(30, (int)result.Status);
        }
        [TestCategory("AddCommentTest")]
        [TestMethod]
        public void EmptyDescriptionAddCommentTest()
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "test";
            commentDto.Description = "";
            commentDto.Score = 1;
            commentDto.UserId = 4;

            var result = _commentService.AddComment(commentDto, _connectionString);


            Assert.AreEqual(30, (int)result.Status);
        }
        [TestCategory("AddCommentTest")]
        [TestMethod]
        [DataRow(-99)]
        [DataRow(0)]
        [DataRow(int.MaxValue)]
        public void WrongScoreAddCommentTest(int score)
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "test";
            commentDto.Description = "";
            commentDto.Score = score;
            commentDto.UserId = 4;

            var result = _commentService.AddComment(commentDto, _connectionString);


            Assert.AreEqual(30, (int)result.Status);
        }
        [TestCategory("AddCommentTest")]
        [TestMethod]
        public void NullTestCommentTest()
        {

            CommentDto commentDto = null;

            var result = _commentService.AddComment(commentDto, _connectionString);


            Assert.AreEqual(34, (int)result.Status);
        }
        [TestCategory("RemoveCommentTest")]
        [TestMethod]
        public void RemoveCommentTest()
        {

            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = 1;
            commentDto.Title = "Some title";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;
            var resultAddingComment = _commentService.AddComment(commentDto, _connectionString);


            var result = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddingComment.Data }, _connectionString);


            Assert.AreEqual(0, (int)result.Status);
        }
        [TestCategory("CountScore")]
        [TestMethod]
        public void CalculateSpotScore()
        {

            //Arrange
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;

            CommentDto commentDto2 = new CommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId2;
            //ACT
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);
            var resultAddComment2 = _commentService.AddComment(commentDto2, _connectionString);
            int result = _commentService.AsyncCalculateAndUpdateScore(this._touristSpotId, _connectionString).GetAwaiter().GetResult();
            //clean
            var res = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);
            //Assert
            Assert.AreEqual(0, (int)resultAddComment.Status);
            Assert.AreEqual(0, (int)resultAddComment2.Status);
            Assert.AreEqual(0, (int)res.Status);
            Assert.AreEqual(3, result);

        }
        [TestCategory("CountScore")]
        [TestMethod]
        public void CalculateSpotScoreWrongId()
        {
            int wrongId = -99;
            //ACT
            int result = _commentService.AsyncCalculateAndUpdateScore(wrongId, _connectionString).GetAwaiter().GetResult();
            //Assert

            Assert.AreEqual(0, result);

        }
        [TestCategory("CountScore")]
        [TestMethod]
        public void CalculateSpotScoreAutomaticCalculationCheck()
        {
            //Arrange
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;

            CommentDto commentDto2 = new CommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId2;
            //Act
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);
            var resultAddComment2 = _commentService.AddComment(commentDto2, _connectionString);
            var result = TouristSpotService.GetTouristSpot(this._touristSpotId, this._connectionString);
            //clean
            var res = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);
            //Assert
            Assert.AreEqual(0, (int)resultAddComment.Status);
            Assert.AreEqual(0, (int)resultAddComment2.Status);
            Assert.AreEqual(0, (int)res.Status);
            Assert.AreEqual(0, (int)result.Status);
            Assert.IsNotNull(result.Data);
            TouristSpot touristSpot = (TouristSpot)result.Data;
            Assert.AreEqual(3, touristSpot.Score);
        }
        [TestMethod]
        public void AddCommentForComment()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            CommentForCommentDto commentDto2 = new CommentForCommentDto();

            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;

            var resultAddComment2 = _commentService.AddCommentForComment(commentDto2, _connectionString);

            Assert.IsNotNull(resultAddComment2);
            Assert.AreEqual(0, (int)resultAddComment2.Status);

        }
        [TestMethod]
        public void AddCommentForCommentNull()
        {
            CommentForCommentDto commentDto2 = null;

            var resultAddComment2 = _commentService.AddCommentForComment(commentDto2, _connectionString);

            Assert.IsNotNull(resultAddComment2);
            Assert.AreEqual(34, (int)resultAddComment2.Status);

        }
        [TestMethod]
        public void EditCommentTestValid()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            CommentDto editCommentDto = new CommentDto();
            editCommentDto.TouristSpotId = this._touristSpotId;
            editCommentDto.Title = "TestEdit";
            editCommentDto.Description = "testDescriptionEdit";
            editCommentDto.Score = 3;
            editCommentDto.UserId = this._userId;
            editCommentDto.Id = (int)resultAddComment.Data;
            var result = _commentService.EditComment(editCommentDto, _connectionString);

            Assert.AreEqual(0, (int)result.Status);

            _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data }, _connectionString);
        }
        [TestMethod]
        public void EditCommentTestNull()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            CommentDto editCommentDto = null;

            var result = _commentService.EditComment(editCommentDto, _connectionString);

            Assert.AreEqual(34, (int)result.Status);

            _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data }, _connectionString);
        }

        [TestMethod]
        public void ValidateCommentParts()
        {
            string title = "tytuł";
            string description = "description";
            int score = 1;
            var resultValidate = CommentService.ValidateComment(title, description, score);
            Assert.IsTrue(resultValidate);

        }
    }
}
