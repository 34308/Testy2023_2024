using JJ_API.Models;
using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components.Forms;
using JJ_API.Interfaces;

namespace JJ_API.Service.Buisneess
{
    public class CommentService : ICommentServiceWrapper
    {
        private ICommentRepository _commentRepository;
        private INotificationService _notificationService;
        public CommentService(INotificationService notificationService)
        {
            this._commentRepository = new CommentRepository();
            _notificationService = notificationService;
        }
        public CommentService(ICommentRepository commentRepository, INotificationService notificationService)
        {
            this._commentRepository = commentRepository;
            _notificationService = notificationService;
        }
        public async Task<int> AsyncCalculateAndUpdateScore(int placeId, string connectionString)
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
        private int CalculateTouristSpotScore(int placeId, SqlConnection connection)
        {
            var scores = _commentRepository.GetCommentScores(placeId, connection);
            try
            {
                int score = scores.Sum() / scores.Count;
                return score;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private bool UpdateTouristSpotScore(int placeId, int score, SqlConnection connection)
        {
            try
            {
                _commentRepository.UpdatePlaceScore(placeId, score, connection);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public ApiResult<Results, object> GetCommentsForTouristSpot(int id, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var response = _commentRepository.CheckIfTouristSpotExist(id, connection);
                    if (response <= 0)
                    {
                        return Response(Results.NotFoundAnyTouristSpots);
                    }
                    List<Comment> comments = _commentRepository.GetCommentsForTouristSpot(id, connection);
                    foreach (var comment in comments)
                    {
                        comment.CommentChildNumber = _commentRepository.GetNumberOfChildComments(comment.Id, connection);
                    }
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public ApiResult<Results, object> GetCommentsForParent(int id, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (0 >= _commentRepository.CheckIfParentExist(id, connection))
                    {
                        return Response(Results.CommentNofound);
                    }
                    List<Comment> comments = _commentRepository.GetCommentsForParent(id, connection);
                    foreach (var comment in comments)
                    {
                        comment.CommentChildNumber = _commentRepository.GetNumberOfChildComments(comment.Id, connection);
                    }
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public ApiResult<Results, object> CheckCommentContent(CommentDto input)
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
        public ApiResult<Results, object> AddComment(CommentDto input, string connectionString)
        {
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
                        int checkIfCanBeInserted = _commentRepository.CountCommentsForSpot(input, connection, transaction);
                        if (checkIfCanBeInserted > 0)
                        {
                            return Response(Results.ScoreHasBeenAlreadyAdded);
                        }
                        int result = _commentRepository.InsertComment(input, connection, transaction);
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

        public ApiResult<Results, object> AddCommentForComment(CommentForCommentDto input, string connectionString)
        {
            int id = 0;
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

                        id = _commentRepository.InsertComment(input, connection, transaction);
                        if (id == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewComment);
                        }


                        if (input.ParentCommentId != 0)
                        {
                            var resopnse = _notificationService.CreateNotificationForCommenting(NotificationService.Notifications.SomeoneCommentYourComment, input.ParentCommentId, connection, transaction, input.UserId);
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
        public ApiResult<Results, object> EditComment(CommentDto input, string connectionString)
        {
            try
            {
                if (input == null)
                {
                    return Response(Results.InputIsNull);
                }
                if (!ValidateComment(input.Title, input.Description)) { return Response(Results.CommentNotValid); }
                ApiResult<Results, object> checkResult = CheckCommentContent(input);
                if (checkResult.Status != 0) { return checkResult; }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        int result = _commentRepository.UpdateComment(input,connection,transaction);
                        if (result == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewComment);
                        }
                        transaction.Commit();
                        this.AsyncCalculateAndUpdateScore(input.TouristSpotId, connectionString);
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
            int minimalScore = 1;
            int maximalScore = 5;
            if (score < minimalScore || score > maximalScore)
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
        public static bool ValidateComment(string title, string comment, int score)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(comment) || !ValidateScore(score))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool CheckSpamPossibility(int userId, SqlConnection connection)
        {
            try
            {
                int howMuchNewCommentsInLast2Minutes = _commentRepository.CountUserCommentsFromLast2Min(userId, connection);
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
        public ApiResult<Results, object> RemoveComment(int userId, List<int> Ids, string connectionString)
        {
            int commentorId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (int id in Ids)
                        {
                            int result = _commentRepository.DeleteComment(id, connection, transaction);
                            if (result == 0)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringRemovingComments);
                            }
                            commentorId = _commentRepository.GetUserIdFromComment(id, connection, transaction);
                            if (userId != commentorId)
                            {
                                var resopnse = _notificationService.CreateNotificationForDeleting(NotificationService.Notifications.YourCommentHasBeenDeleted, id, commentorId, connection, transaction);
                                if (resopnse.Status != 0)
                                {
                                    transaction.Rollback();
                                    return Response(Results.ErrorDuringSendingNotification);
                                }
                            }
                            (int tsid, int pcid) = _commentRepository.GetParentAndTouristSpotId(id, connection, transaction);
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
        public ApiResult<Results, object> GetAllComments(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Comment> comments = _commentRepository.GetAllComments(connection);
                    return Response(Results.OK, comments);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public ApiResult<Results, object> GetAllCommentsForUser(int id, string connectionString)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var response = _commentRepository.CheckIfUserExist(id, connection);
                    if (response != id || response <= 0)
                    {
                        return Response(Results.UserNotFound);
                    }
                    List<Comment> comments = _commentRepository.GetAllCommentsForUser(id, connection);
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
