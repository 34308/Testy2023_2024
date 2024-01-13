using Dapper;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.Data.SqlClient;

namespace JJ_API.Interfaces
{
    public interface ICommentRepository
    {
        public List<int> GetCommentScores(int placeId, SqlConnection connection);

        public int UpdatePlaceScore(int placeId,int score, SqlConnection connection);
        public int CheckIfTouristSpotExist(int id, string connectionString);
        public int CheckIfParentExist(int id, string connectionString);
        public int GetNumberOfChildComments(int parentId, string connectionString);
        public List<Comment> GetCommentsForTouristSpot(int id, string connectionString);
        public List<Comment> GetCommentsForParent(int id, string connectionString);
        public int CountCommentsForSpot(CommentDto comment,SqlConnection connection,SqlTransaction transaction);
        public int InsertComment(CommentDto comment,SqlConnection connection,SqlTransaction transaction);
        public int InsertCommentForComment(CommentForCommentDto comment, SqlConnection connection, SqlTransaction transaction);

        public int CountUserCommentsFromLast2Min(int userId, SqlConnection connection);
        public int DeleteComment(int id, SqlConnection connection, SqlTransaction transaction);
        public int GetUserIdFromComment(int id, SqlConnection connection, SqlTransaction transaction);
        public (int, int) GetParentAndTouristSpotId(int id, SqlConnection connection, SqlTransaction transaction);
        public List<Comment> GetAllComments(string connectionString);
        public List<Comment>  GetAllCommentsForUser(int userId,string connectionString);
        public int CheckIfUserExist(int userId, string connectionString);
        public int UpdateComment(CommentDto input,SqlConnection connection,SqlTransaction transaction);
    }
}
