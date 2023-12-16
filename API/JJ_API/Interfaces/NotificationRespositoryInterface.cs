using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;
using static JJ_API.Service.Buisneess.NotificationService;

namespace JJ_API.Interfaces
{
    public interface NotificationRespositoryInterface
    {
        public List<NotificationDao>GetNotificationForUser(int userId, string connectionString);
        public int GetUserId(int parentCommentId, SqlConnection connection, SqlTransaction transaction);
        public string GetCommentTitle(int parentCommentId, SqlConnection connection, SqlTransaction transaction);
        public int SetNotificatioToChecked(int userId, int notificationId, string connectionString);
        public int InsertNotification(int userId, string message, SqlTransaction transaction, SqlConnection connection);
        public string GetUserLogin(int commentorId, SqlConnection connection, SqlTransaction transaction);
        public int UpdateNotificationForId(int userId, int notificationId, string connectionString);
    }
}
