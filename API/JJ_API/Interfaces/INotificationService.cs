

using JJ_API.Models;
using Microsoft.Data.SqlClient;
using static JJ_API.Service.Buisneess.NotificationService;
using Results = JJ_API.Service.Buisneess.Results;
namespace JJ_API.Interfaces
{
    public interface INotificationService
    {
        public ApiResult<Results, object> GetNotificationForUser(int userId, string connectionString);
        public ApiResult<Results, object> SetNotificatioToChecked(int userId, int notificationId, string connectionString);
        internal ApiResult<Results, object> CreateNotificationForCommenting(Notifications notification, int parentCommentId, SqlConnection connection, SqlTransaction transaction, int commentorId = 0);
        internal ApiResult<Results, object> CreateNotificationForDeleting(Notifications notification, int commentId, int userId, SqlConnection connection, SqlTransaction transaction);
    }
}
