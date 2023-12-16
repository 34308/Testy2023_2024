using Dapper;
using JJ_API.Interfaces;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Service.Buisneess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Transactions;
using static JJ_API.Service.Buisneess.NotificationService;

namespace JJ_API
{
    public class NotificationRespository : NotificationRespositoryInterface
    {

        public int InsertNotification(int userId, string message, SqlTransaction transaction, SqlConnection connection)
        {
            string q_insertNotification = "INSERT INTO [Notification] ([UserId],[Description],[CreatedOn],[Checked]) OUTPUT Inserted.Id VALUES (@userid,@description,@date,0)";
            return connection.QueryFirstOrDefault<int>(q_insertNotification, new { userid = userId, description = message, date = DateTime.Now }, transaction);
        }
        public int UpdateNotificationForId(int userId,int notificationId,string connectionString)
        {
            string q_getNotificationForId = "UPDATE Notification SET Checked = 1  WHERE UserId = @userid AND id = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<int>(q_getNotificationForId, new { userid = userId, id = notificationId });
            }
        }
        public List<NotificationDao> GetNotificationForUser(int userId, string connectionString)
        {
            string q_getNotification = "SELECT * FROM Notification WHERE UserId = @userid AND Checked = 0";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                List<NotificationDao> notifications = connection.Query<NotificationDao>(q_getNotification, new { userid = userId }).ToList();
                return notifications;
            }
        }
        public int GetUserId(int parentCommentId, SqlConnection connection, SqlTransaction transaction)
        {
            string getUserId = "Select [UserId] FROM [Comment] WHERE Id=@id";

            return connection.QueryFirstOrDefault<int>(getUserId, new { id = parentCommentId }, transaction);

        }
        public string GetCommentTitle(int parentCommentId, SqlConnection connection, SqlTransaction transaction)
        {
            string getCommentTitle = "Select [Title] FROM [Comment] WHERE Id=id";

            return connection.QueryFirstOrDefault<string>(getCommentTitle, new { id = parentCommentId }, transaction);

        }

        public int SetNotificatioToChecked(int userId, int notificationId, string connectionString)
        {
            return 0;
        }

        public string GetUserLogin(int commentorId, SqlConnection connection, SqlTransaction transaction)
        {
            string getUserLogin = "Select [Login] FROM [User] WHERE id=@userid";

            return connection.QueryFirstOrDefault<string>(getUserLogin, new { userid = commentorId }, transaction);
        }
    }
}
