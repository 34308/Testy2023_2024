using Dapper;
using JJ_API.Interfaces;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service.Buisneess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JJ_API.Tests
{
    [TestClass]
    public class NotificationServiceTest
    {
        private string _connectionString = "Data Source=\"localhost\\sqljj\";Initial Catalog=JJDBTests;User=sa;Password = 5540;Persist Security Info=True;Pooling=False;TrustServerCertificate=true";
        private TestContext _testContextInstance;
        private int _userId = 0;
        private int _userId2 = 0;
        private int _touristSpotId = 0;
        NotificationService _notificationService;
        Mock<NotificationRespositoryInterface> _repositoryMock;
        private CommentService _commentService;
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }
        [TestInitialize]
        public void Prepare()
        {
            _notificationService = new NotificationService();
            _repositoryMock = new Mock<NotificationRespositoryInterface>();
            _commentService = new CommentService(_notificationService);
            RegisterDto registerDto = new RegisterDto();
            registerDto.AvatarId = 1;
            registerDto.Email = "test@wp.pl";
            registerDto.Login = "TestLogin";
            registerDto.Password = "Password123@";

            RegisterDto registerDto2 = new RegisterDto();
            registerDto2.AvatarId = 2;
            registerDto2.Email = "test232@wp.pl";
            registerDto2.Login = "Test22Login";
            registerDto2.Password = "Password123@";

            var responseForAddingUser = RegistrationService.RegisterUser(registerDto, _connectionString);

            var responseForAddingUser2 = RegistrationService.RegisterUser(registerDto2, _connectionString);
            this._userId = (int)responseForAddingUser.Data;
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
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute("TRUNCATE TABLE [Notification]");
            }

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void TestCreateNotificationImpl()
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

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = _notificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection, transaction, this._userId);
                transaction.Commit();

                Assert.AreEqual(0, (int)result.Status);
            }
            var res = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void TestCreateNotificationJed()
        {

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                _repositoryMock.Setup(repo => repo.GetUserLogin(this._userId, connection, transaction))
                      .Returns("Login");
                _repositoryMock.Setup(repo => repo.GetCommentTitle(1, connection, transaction))
                      .Returns("Tytuł");
                _repositoryMock.Setup(repo => repo.GetUserId(1, connection, transaction))
                    .Returns(2);
                _repositoryMock.Setup(repo => repo.InsertNotification(2, "Twój komentarz Tytuł został skomentowany przez: Login", transaction, connection))
                    .Returns(1);
                _notificationService = new NotificationService(_repositoryMock.Object);
                var result = _notificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, 1, connection, transaction, this._userId);

                _repositoryMock.Verify(repo => repo.GetUserLogin(this._userId, connection, transaction), Times.Once);
                _repositoryMock.Verify(repo => repo.GetCommentTitle(1, connection, transaction), Times.Once);
                _repositoryMock.Verify(repo => repo.GetUserId(1, connection, transaction), Times.Once);
                _repositoryMock.Verify(repo => repo.InsertNotification(2, "Twój komentarz Tytuł został skomentowany przez: Login", transaction, connection), Times.Once);

                Assert.AreEqual(0, (int)result.Status);
            }

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void GetNotificationForUser()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId2;
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            CommentForCommentDto commentDto2 = new CommentForCommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;
            var resultAddComment2 = _commentService.AddCommentForComment(commentDto2, _connectionString);

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = _notificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection, transaction, this._userId);
                transaction.Commit();
                Assert.AreEqual(0, (int)result.Status);

            }
            var response = _notificationService.GetNotificationForUser(this._userId2, _connectionString);
            Assert.IsNotNull(response);
            Assert.AreEqual(0, (int)response.Status);
            Assert.AreEqual(this._userId2, ((List<NotificationDao>)response.Data)[0].UserId);

            var res = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void GetNotificationForUserJ()
        {
            NotificationDao notificationDao = new NotificationDao();
            notificationDao.UserId = this._userId;
            notificationDao.Checked = false;
            notificationDao.Description = "test";
            notificationDao.CreatedOn = DateTime.Now;
            _repositoryMock.Setup(repo => repo.GetNotificationForUser(this._userId, _connectionString))
                     .Returns(new List<NotificationDao>() { notificationDao });

            _notificationService = new NotificationService(_repositoryMock.Object);
            var response = _notificationService.GetNotificationForUser(this._userId, _connectionString);
            _repositoryMock.Verify(repo => repo.GetNotificationForUser(this._userId, _connectionString), Times.Once);
            Assert.IsNotNull(response);
            Assert.AreEqual(0, (int)response.Status);
            Assert.AreEqual(this._userId, ((List<NotificationDao>)response.Data)[0].UserId);

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void SetNotificationForChecked()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId2;
            var resultAddComment = _commentService.AddComment(commentDto, _connectionString);

            CommentForCommentDto commentDto2 = new CommentForCommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;
            var resultAddComment2 = _commentService.AddCommentForComment(commentDto2, _connectionString);

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = _notificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection, transaction, this._userId);
                transaction.Commit();
                Assert.AreEqual(0, (int)result.Status);
                var resultForChecking = _notificationService.SetNotificatioToChecked(this._userId2, (int)result.Data, _connectionString);
                Assert.AreEqual(0, (int)resultForChecking.Status);

            }

            var res = _commentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void SetNotificationForCheckedJ()
        {

            _repositoryMock.Setup(repo => repo.SetNotificatioToChecked(this._userId2, 1, _connectionString))
                .Returns(1);

            var resultForChecking = _notificationService.SetNotificatioToChecked(this._userId2, 1, _connectionString);
            Assert.AreEqual(0, (int)resultForChecking.Status);
        }
    }


}
