using Dapper;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace JJ_API.Interfaces
{
    public class CommentRepository : ICommentRepository
    {
        public int CheckIfParentExist(int id, SqlConnection connection)
        {
            string q_checkIfParentExist = "SELECT id FROM Comment WHERE Id=@id";
            return connection.QueryFirstOrDefault<int>(q_checkIfParentExist, new { id = id });
        }

        public int CheckIfTouristSpotExist(int id, SqlConnection connection)
        {
            string q_checkIfTouristSpotExists = "SELECT id FROM TouristSpot WHERE id=@id";
            return connection.QueryFirstOrDefault<int>(q_checkIfTouristSpotExists, new { id = id });
        }

        public int CheckIfUserExist(int userId, SqlConnection connection)
        {
            string q_checkUser = "SELECT id FROM [User] WHERE Id=@id";
            return connection.QueryFirstOrDefault<int>(q_checkUser, new { id = userId }); 
        }

        public int CountCommentsForSpot(CommentDto input, SqlConnection connection, SqlTransaction transaction)
        {
            string q_checkIfCanBeInserted = "SELECT Count(Id) FROM Comment WHERE UserId=@userid AND Score <> 0 AND Score IS NOT NULL AND (ParentCommentId IS NULL OR ParentCommentId=0)  AND TouristSpotId =@tsid";
            return connection.QueryFirstOrDefault<int>(q_checkIfCanBeInserted, new { userid = input.UserId, tsid = input.TouristSpotId }, transaction); 
        }

        public int CountUserCommentsFromLast2Min(int userId, SqlConnection connection)
        {
            string q_countLast2MinComments = "SELECT COUNT(userId) AS UserCount " +
                "FROM [Comment] " +
                "WHERE DATEDIFF(MINUTE, CreatedAt, @targetDate) BETWEEN -1 AND 1" +
                "AND userId = @userid;";
            DateTime createdTime = DateTime.Now;
            return connection.QueryFirstOrDefault<int>(q_countLast2MinComments, new { userid = userId, targetDate = createdTime });
        }

        public int DeleteComment(int id, SqlConnection connection, SqlTransaction transaction)
        {
            string q_removeCommentForSpot = "DELETE FROM Comment WHERE Id=@id";
            return connection.Execute(q_removeCommentForSpot, new { id = id }, transaction);
        }

        public List<Comment> GetAllComments(SqlConnection connection)
        {
            string q_getComments = "SELECT * FROM Comment";

            return connection.Query<Comment>(q_getComments).ToList();
        }

        public List<Comment> GetAllCommentsForUser(int userId, SqlConnection connection)
        {
            string q_getComments = "SELECT * FROM [Comment] WHERE UserId=@id";
            return connection.Query<Comment>(q_getComments, new { id = userId }).ToList(); 
        }

        public List<int> GetCommentScores(int placeId, SqlConnection connection)
        {
            string q_getAllScores = "SELECT [Score] FROM Comment WHERE TouristSpotId=@id AND Score<>0 AND SCORE IS NOT NULL ";
            List<int> scores = connection.Query<int>(q_getAllScores, new { id = placeId }).ToList();
            return scores;
        }

        public List<Comment> GetCommentsForParent(int id, SqlConnection connection)
        {
            string q_getCommentForParrent = "SELECT cm.Id,Title,cm.Description,cm.Score,cm.UserId,cm.TouristSpotId,cm.CreatedAt ,av.Picture as Avatar,us.[Login] AS Username FROM " +
               "Comment cm JOIN [User] us ON us.Id=cm.UserId JOIN Avatar av ON us.AvatarId = av.Id  " +
              " WHERE ParentCommentId = @id";
            return connection.Query<Comment>(q_getCommentForParrent, new { id = id }).ToList();
        }

        public List<Comment> GetCommentsForTouristSpot(int id, SqlConnection connection)
        {
            string q_getCommentsForTouristSpot = "SELECT cm.Id,Title,cm.Description,cm.Score,cm.UserId,cm.TouristSpotId,cm.CreatedAt ,av.Picture as Avatar,us.[Login] AS Username FROM " +
               "Comment cm JOIN [User] us ON us.Id=cm.UserId JOIN Avatar av ON us.AvatarId = av.Id  " +
              " WHERE TouristSpotId = @id AND ([ParentCommentId] IS NULL OR [ParentCommentId] = 0)";
            return connection.Query<Comment>(q_getCommentsForTouristSpot, new { id = id }).ToList();
        }

        public int GetNumberOfChildComments(int parentId, SqlConnection connection)
        {
            string q_getNumberOfChildrenForComment = "SELECT COUNT(cm.Id) FROM " +
                    "Comment cm" +
                   " WHERE  [ParentCommentId] = @parentid";
            return connection.QueryFirstOrDefault<int>(q_getNumberOfChildrenForComment, new { parentid = parentId });
        }

        public (int, int) GetParentAndTouristSpotId(int id, SqlConnection connection, SqlTransaction transaction)
        {
            string q_touristSpotIdAndParentCommentId = "SELECT [TouristSpotId],[ParentCommentId] FROM [Comment] WHERE Id=@id";
            return connection.QueryFirstOrDefault<(int, int)>(q_touristSpotIdAndParentCommentId, new { id = id }, transaction);
        }

        public int GetUserIdFromComment(int id, SqlConnection connection, SqlTransaction transaction)
        {
            string q_getUserId = "SELECT UserId FROM Comment WHERE Id=@id";
            return connection.QueryFirstOrDefault<int>(q_getUserId, new { id = id }, transaction);
        }

        public int InsertComment(CommentDto input, SqlConnection connection, SqlTransaction transaction)
        {
            string q_insertComment = "INSERT INTO Comment (Title,Description,Score,UserId,TouristSpotId,CreatedAt,ParentCommentId ) " +
              "OUTPUT INSERTED.Id VALUES (@title,@description,@score,@userid,@touristspotid,@date,@commentforcommentid) ";
            

            return connection.QueryFirstOrDefault<int>(q_insertComment, new { title = input.Title, description = input.Description, score = input.Score, userid = input.UserId, touristspotid = input.TouristSpotId, date = DateTime.Now, commentforcommentid = 0 }, transaction);
        }

        public int UpdateComment(CommentDto input, SqlConnection connection, SqlTransaction transaction)
        {
            string q_updateComment = "UPDATE Comment SET Title=@title,Description=@description,Score=@score,UpdatedAt=@date WHERE Id=@id";
            return connection.Execute(q_updateComment, new { title = input.Title, description = input.Description, score = input.Score, date = DateTime.Now, id = input.Id }, transaction);
        }

        public int UpdatePlaceScore(int placeId, int score, SqlConnection connection)
        {
            string q_updateScore = "UPDATE TouristSpot SET Score=@score WHERE Id=@id";
            return connection.Execute(q_updateScore, new { id = placeId, score = score });
        }
    }
}
