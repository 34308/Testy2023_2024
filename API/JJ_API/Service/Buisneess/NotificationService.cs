using Dapper;
using JJ_API.Models;
using JJ_API.Models.DTO;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using JJ_API.Models.DAO;

namespace JJ_API.Service.Buisneess
{
    public class NotificationService
    {
        public static ApiResult<Results, object> GetNotificationForUser(int userId, string connectionString)
        {
            string q_getNotification = "SELECT * FROM Notification WHERE UserId = @userid AND Checked = 0";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<NotificationDao> notifications = connection.Query<NotificationDao>(q_getNotification, new { userid = userId }).ToList();
                    return Response(Results.OK, notifications);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> SetNotificatioToChecked(int userId, int notificationId, string connectionString)
        {
            string q_getNotificationForId = "UPDATE Notification SET Checked = 1  WHERE UserId = @userid AND id = @id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<NotificationDao> notifications = connection.Query<NotificationDao>(q_getNotificationForId, new { userid = userId, id = notificationId }).ToList();
                    return Response(Results.OK, notifications);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public enum Notifications
        {
            SomeoneCommentYourComment,
            YourCommentHasBeenDeleted,
            YourCommentHasBeenEdited,


        }
        internal static ApiResult<Results, object> CreateNotificationForCommenting(Notifications notification, int parentCommentId, SqlConnection connection, SqlTransaction transaction,int commentorId = 0)
        {
            string q_insertNotification = "INSERT INTO [Notification] VALUES (@userid,@description,@date,0)";
            string message = "";
            string getUserId = "Select [UserId] FROM [Comment] WHERE Id=@id";

            string getUserLogin = "Select [Login] FROM [User] WHERE id=@userid";
            string getCommentTitle = "Select [Title] FROM [Comment] WHERE Id=id";
            string commentorLogin = "";
            string commentTitle = "";
            int userId = 0;
            try
            {
              
                if (commentorId != 0)
                {
                    commentorLogin = connection.QueryFirstOrDefault<string>(getUserLogin, new { userid = commentorId }, transaction);
                }
                commentTitle = connection.QueryFirstOrDefault<string>(getCommentTitle, new { id =  parentCommentId }, transaction);

                userId = connection.QueryFirstOrDefault<int>(getUserId, new { id = parentCommentId }, transaction);

                message = GetMessage(notification, message, commentorLogin, commentTitle);

                int response = connection.Execute(q_insertNotification, new { userid = userId, description = message, date = DateTime.Now },transaction);
                if (response == 1)
                {
                    return Response(Results.OK);
                }
                else
                {
                    return Response(Results.NotInsertedNotification);
                }

            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        internal static ApiResult<Results, object> CreateNotificationForDeleting(Notifications notification,int commentId,int userId, SqlConnection connection, SqlTransaction transaction)
        {
            string q_insertNotification = "INSERT INTO [Notification] VALUES (@userid,@description,@date,0)";
            string message = "";
            string getCommentTitle = "Select [Title] FROM [Comment] WHERE Id=id";
            string commentorLogin = "";
            string commentTitle = "";
            try
            {
               
                commentTitle = connection.QueryFirstOrDefault<string>(getCommentTitle, new { id = commentId }, transaction);
                message = GetMessage(notification, message, commentorLogin, commentTitle);
                int response = connection.Execute(q_insertNotification, new { userid = userId, description = message, date = DateTime.Now }, transaction);
                if (response == 1)
                {
                    return Response(Results.OK);
                }
                else
                {
                    return Response(Results.NotInsertedNotification);
                }

            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }

        private static string GetMessage(Notifications notification, string message, string commentorLogin, string commentTitle)
        {
            switch (notification)
            {
                case Notifications.SomeoneCommentYourComment:
                    message = $"Twój komentarz {commentTitle} został skomentowany przez: {commentorLogin}";
                    break;
                case Notifications.YourCommentHasBeenDeleted:
                    message = $"Twój komentarz {commentTitle} został usunięty przez administratora";
                    break;
                case Notifications.YourCommentHasBeenEdited:
                    message = $"Twój komentarz {commentTitle} został zmieniony przez administratora";
                    break;
            }

            return message;
        }

        public static ApiResult<Results, object> Response(Results results, List<NotificationDao> notifications)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, notifications);
        }
        public static ApiResult<Results, object> Response(Results results, string error)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, error);
        }
        public static ApiResult<Results, object> Response(Results results, NotificationDao notification)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, notification);
        }
        public static ApiResult<Results, object> Response(Results results)
        {
            string message = results switch
            {
                Results.OK => "OK.",
                Results.GeneralError => "Error has occured.",
                Results.ErrorDuringAddingNewTouristSpots => "Error. One of Tourist Spots have been not added",
                Results.ErrorDuringRemovingTouristSpots => "Error. One of Tourist Spots have been not deleted",
                Results.NotFoundAnyTouristSpots => "Error. No tourist spots found",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }
    }
}
