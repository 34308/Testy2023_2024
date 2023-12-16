using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Service.Buisneess;
using Results = JJ_API.Service.Buisneess.Results;

namespace JJ_API.Interfaces
{
    public interface ICommentServiceWrapper
    {
        Task<int> AsyncCalculateAndUpdateScore(int placeId, string connectionString);
        public ApiResult<Results, object> GetCommentsForTouristSpot(int id, string connectionString);
        public ApiResult<Results, object> GetCommentsForParent(int id, string connectionString);
        public ApiResult<Results, object> CheckCommentContent(CommentDto input);
        public ApiResult<Results, object> AddComment(CommentDto input, string connectionString);
        public  ApiResult<Results, object> AddCommentForComment(CommentForCommentDto input, string connectionString);
        public ApiResult<Results, object> EditComment(CommentDto input, string connectionString);
        public ApiResult<Results, object> RemoveComment(int userId, List<int> Ids, string connectionString);
        public  ApiResult<Results, object> GetAllComments(string connectionString);
        public  ApiResult<Results, object> GetAllCommentsForUser(int id, string connectionString);

    }



}
