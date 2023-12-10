using Dapper;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service.Buisneess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }
        [TestInitialize]
        public void Prepare()
        {
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
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute("TRUNCATE TABLE [Notification]");
            }

        }
        [TestCategory("testNotification")]
        [TestMethod]
        public void TestCreateNotification()
        {
            CommentDto commentDto = new CommentDto();
            commentDto.TouristSpotId = this._touristSpotId;
            commentDto.Title = "Test";
            commentDto.Description = "testDescription";
            commentDto.Score = 1;
            commentDto.UserId = this._userId;

            var resultAddComment = CommentService.AddComment(commentDto, _connectionString);
            CommentForCommentDto commentDto2 = new CommentForCommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;
            var resultAddComment2 = CommentService.AddCommentForComment(commentDto2, _connectionString);

            using (SqlConnection connection=new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = NotificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection,transaction,this._userId);
                transaction.Commit();

                Assert.AreEqual(0, (int)result.Status);
            }
            var res = CommentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

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
            var resultAddComment = CommentService.AddComment(commentDto, _connectionString);

            CommentForCommentDto commentDto2 = new CommentForCommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;
            var resultAddComment2 = CommentService.AddCommentForComment(commentDto2, _connectionString);

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = NotificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection, transaction, this._userId);
                transaction.Commit();
                Assert.AreEqual(0, (int)result.Status);

            }
            var response=NotificationService.GetNotificationForUser(this._userId2,_connectionString);
            Assert.IsNotNull(response);
            Assert.AreEqual(0,(int) response.Status);
            Assert.AreEqual(this._userId2, ((List<NotificationDao>)response.Data)[0].UserId);

            var res = CommentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

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
            var resultAddComment = CommentService.AddComment(commentDto, _connectionString);

            CommentForCommentDto commentDto2 = new CommentForCommentDto();
            commentDto2.TouristSpotId = this._touristSpotId;
            commentDto2.Title = "Test";
            commentDto2.Description = "testDescription";
            commentDto2.Score = 5;
            commentDto2.UserId = this._userId;
            commentDto2.ParentCommentId = (int)resultAddComment.Data;
            var resultAddComment2 = CommentService.AddCommentForComment(commentDto2, _connectionString);

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var result = NotificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, (int)resultAddComment.Data, connection, transaction, this._userId);
                transaction.Commit();
                Assert.AreEqual(0, (int)result.Status);
                var resultForChecking= NotificationService.SetNotificatioToChecked(this._userId2,(int) result.Data,_connectionString);
                Assert.AreEqual(0, (int)resultForChecking.Status);

            }
           

            var res = CommentService.RemoveComment(this._userId, new List<int> { (int)resultAddComment.Data, (int)resultAddComment2.Data }, _connectionString);

        }
    }


}
