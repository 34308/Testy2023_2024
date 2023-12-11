using JJ_API.Models;
using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace JJ_API.Service.Buisneess
{
    public static class CommentService
    {
        public static async Task<int> AsyncCalculateAndUpdateScore(int placeId, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int spotscore = CalculateTouristSpotScore(placeId, connection);
                if (spotscore > 0)
                {
                    UpdateTouristSpotScore(placeId, spotscore, connection);
                    return spotscore;
                }
            }
            return 0;
        }
        private static int CalculateTouristSpotScore(int placeId, SqlConnection connection)
        {
            string q_getAllScores = "SELECT [Score] FROM Comment WHERE TouristSpotId=@id AND Score<>0 AND SCORE IS NOT NULL ";
            try
            {
                List<int> scores = connection.Query<int>(q_getAllScores, new { id = placeId }).ToList();
                int score = scores.Sum() / scores.Count;
                return score;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private static bool UpdateTouristSpotScore(int placeId, int score, SqlConnection connection)
        {
            string q_updateScore = "UPDATE TouristSpot SET Score=@score WHERE Id=@id";
            try
            {
                connection.Execute(q_updateScore, new { id = placeId, score = score });

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static ApiResult<Results, object> GetCommentsForTouristSpot(int id, string connectionString)
        {
            string q_getCommentsForTouristSpot = "SELECT cm.Id,Title,cm.Description,cm.Score,cm.UserId,cm.TouristSpotId,cm.CreatedAt ,av.Picture as Avatar,us.[Login] AS Username FROM " +
                "Comment cm JOIN [User] us ON us.Id=cm.UserId JOIN Avatar av ON us.AvatarId = av.Id  " +
               " WHERE TouristSpotId = @id AND ([ParentCommentId] IS NULL OR [ParentCommentId] = 0)";
            string q_getNumberOfChilderForComment = "SELECT COUNT(cm.Id) FROM " +
                "Comment cm" +
               " WHERE  [ParentCommentId] = @parentid";
            string q_checkIfTouristSpotExists = "SELECT id FROM TouristSpot WHERE id=@id";
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var response = connection.QueryFirstOrDefault<int>(q_checkIfTouristSpotExists, new { id = id });
                    if (response <= 0)
                    {
                        return Response(Results.NotFoundAnyTouristSpots);
                    }
                    List<Comment> comments = connection.Query<Comment>(q_getCommentsForTouristSpot, new { id = id }).ToList();
                    foreach (var comment in comments)
                    {
                        comment.CommentChildNumber = connection.QueryFirstOrDefault<int>(q_getNumberOfChilderForComment, new { parentid = comment.Id });
                    }
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetCommentsForParent(int id, string connectionString)
        {
            string q_getCommentForParrent = "SELECT cm.Id,Title,cm.Description,cm.Score,cm.UserId,cm.TouristSpotId,cm.CreatedAt ,av.Picture as Avatar,us.[Login] AS Username FROM " +
                "Comment cm JOIN [User] us ON us.Id=cm.UserId JOIN Avatar av ON us.AvatarId = av.Id  " +
               " WHERE ParentCommentId = @id";
            string q_getNumberOfChilderForComment = "SELECT COUNT(cm.Id) FROM " +
               "Comment cm" +
              " WHERE  [ParentCommentId] = @parentid";
            string q_checkIfParentExist = "SELECT id FROM Comment WHERE Id=@id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (0 >= connection.QueryFirstOrDefault<int>(q_checkIfParentExist, new { id = id }))
                    {
                        return Response(Results.CommentNofound);
                    }
                    List<Comment> comments = connection.Query<Comment>(q_getCommentForParrent, new { id = id }).ToList();
                    foreach (var comment in comments)
                    {
                        comment.CommentChildNumber = connection.QueryFirstOrDefault<int>(q_getNumberOfChilderForComment, new { parentid = comment.Id });
                    }
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> CheckCommentContent(CommentDto input)
        {
            (bool isTitleClean, string badTitleWord) = CensorshipService.CheckForCurses(input.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            (bool isDescriptionClean, string badDescription) = CensorshipService.CheckForCurses(input.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (!isTitleClean)
            {
                return Response(Results.CurseFoundInContent, badTitleWord);
            }
            if (!isDescriptionClean)
            {
                return Response(Results.CurseFoundInContent, badDescription);
            }
            return Response(Results.OK);
        }
        public static ApiResult<Results, object> AddComment(CommentDto input, string connectionString)
        {
            string q_checkIfCanBeInserted = "SELECT Count(Id) FROM Comment WHERE UserId=@userid AND Score <> 0 AND Score IS NOT NULL AND (ParentCommentId IS NULL OR ParentCommentId=0)  AND TouristSpotId =@tsid";
            string q_insertComment = "INSERT INTO Comment (Title,Description,Score,UserId,TouristSpotId,CreatedAt,ParentCommentId ) " +
                "OUTPUT INSERTED.Id VALUES (@title,@description,@score,@userid,@touristspotid,@date,@commentforcommentid) ";
            try
            {
                if (input == null)
                {
                    return Response(Results.InputIsNull);
                }
                if (!ValidateComment(input.Title, input.Description)) { return Response(Results.CommentNotValid); }
                if (!ValidateScore(input.Score)) { return Response(Results.CommentNotValid); }
                ApiResult<Results, object> checkResult = CheckCommentContent(input);
                if (checkResult.Status != 0)
                {
                    return checkResult;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (!CheckSpamPossibility(input.UserId, connection))
                    {
                        return Response(Results.TooManyCommentsPast2Min);
                    }
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        int checkIfCanBeInserted = connection.QueryFirstOrDefault<int>(q_checkIfCanBeInserted, new { userid = input.UserId, tsid = input.TouristSpotId });
                        if (checkIfCanBeInserted > 0)
                        {
                            return Response(Results.ScoreHasBeenAlreadyAdded);
                        }
                        int result = connection.QueryFirstOrDefault<int>(q_insertComment, new { title = input.Title, description = input.Description, score = input.Score, userid = input.UserId, touristspotid = input.TouristSpotId, date = DateTime.Now, commentforcommentid = 0 }, transaction);
                        if (result == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewComment);
                        }
                        transaction.Commit();
                        AsyncCalculateAndUpdateScore(input.TouristSpotId, connectionString);
                        return Response(Results.OK, result);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }

        private static ApiResult<Results, object> Response(object inputIsNull)
        {
            throw new NotImplementedException();
        }

        public static ApiResult<Results, object> AddCommentForComment(CommentForCommentDto input, string connectionString)
        {
            int id = 0;
            string q_insertComment = "INSERT INTO Comment (Title,Description,Score,UserId,TouristSpotId,CreatedAt,ParentCommentId ) OUTPUT INSERTED.Id VALUES (@title,@description,@score,@userid,@touristspotid,@date,@commentforcommentid) ";
            try
            {
                if (input == null)
                {
                    return Response(Results.InputIsNull);
                }
                if (!ValidateComment(input.Title, input.Description)) { return Response(Results.CommentNotValid); }
                ApiResult<Results, object> checkResult = CheckCommentContent(input);
                if (checkResult.Status != 0)
                {
                    return checkResult;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (!CheckSpamPossibility(input.UserId, connection))
                    {
                        return Response(Results.TooManyCommentsPast2Min);
                    }
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {

                        id = connection.QueryFirstOrDefault<int>(q_insertComment, new { title = input.Title, description = input.Description, score = input.Score, userid = input.UserId, touristspotid = input.TouristSpotId, date = DateTime.Now, commentforcommentid = input.ParentCommentId }, transaction);
                        if (id == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewComment);
                        }


                        if (input.ParentCommentId != 0)
                        {
                            var resopnse = NotificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, input.ParentCommentId, connection, transaction, input.UserId);
                            if (resopnse.Status != 0)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringSendingNotification);
                            }
                        }
                        transaction.Commit();
                        return Response(Results.OK, id);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> EditComment(CommentDto input, string connectionString)
        {
            string q_updateComment = "UPDATE Comment SET Title=@title,Description=@description,Score=@score,UpdatedAt=@date WHERE Id=@id";
            try
            {
                if (input == null)
                {
                    return Response(Results.InputIsNull);
                }
                if (!ValidateComment(input.Title, input.Description)) { return Response(Results.OK); }
                ApiResult<Results, object> checkResult = CheckCommentContent(input);
                if (checkResult.Status != 0) { return checkResult; }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        int result = connection.Execute(q_updateComment, new { title = input.Title, description = input.Description, score = input.Score, date = DateTime.Now, id = input.Id }, transaction);
                        if (result == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewComment);
                        }
                        transaction.Commit();
                        AsyncCalculateAndUpdateScore(input.TouristSpotId, connectionString);
                        return Response(Results.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        private static bool ValidateScore(int score)
        {
            if (score == 0 || score > 5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool ValidateComment(string title, string comment)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(comment))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool ValidateComment(string title, string comment, int score)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(comment) || ValidateScore(score))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool CheckSpamPossibility(int userId, SqlConnection connection)
        {
            string q_GetLastCreateDate = "SELECT MAX([CreatedAt]) FROM [Comment] WHERE UserId=@userid";

            string q_countLast2MinComments = "SELECT COUNT(userId) AS UserCount " +
                "FROM [Comment] " +
                "WHERE DATEDIFF(MINUTE, CreatedAt, @targetDate) BETWEEN -1 AND 1" +
                "AND userId = @userid;";
            try
            {
                DateTime createdTime = DateTime.Now;
                int howMuchNewCommentsInLast2Minutes = connection.QueryFirstOrDefault<int>(q_countLast2MinComments, new { userid = userId, targetDate = createdTime });
                if (howMuchNewCommentsInLast2Minutes > 5)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static ApiResult<Results, object> RemoveComment(int userId, List<int> Ids, string connectionString)
        {
            int commentorId = 0;
            string q_removeCommentForSpot = "DELETE FROM Comment WHERE Id=@id";
            string q_getUserId = "SELECT UserId FROM Comment WHERE Id=@id";
            string q_touristSpotIdAndParentCommentId = "SELECT [TouristSpotId],[ParentCommentId] FROM [Comment] WHERE Id=@id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (int id in Ids)
                        {
                            int result = connection.Execute(q_removeCommentForSpot, new { id = id }, transaction);
                            if (result == 0)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringRemovingComments);
                            }
                            commentorId = connection.QueryFirstOrDefault<int>(q_getUserId, new { id = id }, transaction);
                            if (userId != commentorId)
                            {
                                var resopnse = NotificationService.CreateNotificationForDeleting(NotificationService.Notifications.YourCommentHasBeenDeleted, id, commentorId, connection, transaction);
                                if (resopnse.Status != 0)
                                {
                                    transaction.Rollback();
                                    return Response(Results.ErrorDuringSendingNotification);
                                }
                            }
                            (int tsid, int pcid) = connection.QueryFirstOrDefault<(int, int)>(q_touristSpotIdAndParentCommentId, new { id = id }, transaction);
                            if (pcid != 0 && pcid != -1)
                            {
                                AsyncCalculateAndUpdateScore(tsid, connectionString);
                            }
                        }
                        transaction.Commit();

                        return Response(Results.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetAllComments(string connectionString)
        {
            string q_getComments = "SELECT * FROM Comment";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Comment> comments = connection.Query<Comment>(q_getComments).ToList();
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetAllCommentsForUser(int id, string connectionString)
        {
            string q_getComments = "SELECT * FROM [Comment] WHERE UserId=@id";
            string q_checkUser = "SELECT id FROM [User] WHERE Id=@id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var response = connection.QueryFirstOrDefault<int>(q_checkUser, new { id = id });
                    if (response != id || response <= 0)
                    {
                        return Response(Results.UserNotFound);
                    }
                    List<Comment> comments = connection.Query<Comment>(q_getComments, new { id = id }).ToList();
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<Comment> comments)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, comments);
        }
        public static ApiResult<Results, object> Response(Results results, string token)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, token);
        }
        public static ApiResult<Results, object> Response(Results results, int id)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, id);
        }
        public static ApiResult<Results, object> Response(Results results)
        {
            string message = results switch
            {
                Results.OK => "OK.",
                Results.GeneralError => "Error has occured.",
                Results.ErrorDuringAddingNewComment => "Error. While adding new Comment",
                Results.ErrorDuringRemovingComments => "Error. While deleting Comments",
                Results.CommentNotValid => "Comment not valid, check all values",
                Results.TooManyCommentsPast2Min => "You can only add few comments per minute",
                Results.ScoreHasBeenAlreadyAdded => "Already added score for this place",
                Results.InputIsNull => "Input is null",
                _ => ""
            };

            return new ApiResult<Results, object>(results, message);
        }
    }

}
