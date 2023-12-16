using JJ_API.Models.DAO;
using JJ_API.Models;
using JJ_API.Service.Buisneess;
using Results = JJ_API.Service.Buisneess.Results;
using JJ_API.Models.DTO;

namespace JJ_API.Interfaces
{
    public class CommentServiceWrapper : ICommentServiceWrapper
    {
        public async Task<int> AsyncCalculateAndUpdateScore(int placeId, string connectionString)
        {
            return await CommentService.AsyncCalculateAndUpdateScore(placeId, connectionString);
        }
        public ApiResult<Results, object> GetCommentsForTouristSpot(int id, string connectionString)
        {
            return CommentService.GetCommentsForTouristSpot(id, connectionString);
        }
        public ApiResult<Results, object> GetCommentsForParent(int id, string connectionString)
        {
            return CommentService.GetCommentsForParent(id, connectionString);
        }
        public ApiResult<Results, object> CheckCommentContent(CommentDto input)
        {
            return CommentService.CheckCommentContent(input);
        }
        public ApiResult<Results, object> AddComment(CommentDto input, string connectionString) {
            return CommentService.AddComment( input,connectionString);
        }
        public ApiResult<Results, object> AddCommentForComment(CommentForCommentDto input, string connectionString) {
            return CommentService.AddCommentForComment( input, connectionString);
        }
        public ApiResult<Results, object> EditComment(CommentDto input, string connectionString) {
            return CommentService.EditComment(input, connectionString);
                }
        public ApiResult<Results, object> RemoveComment(int userId, List<int> ids, string connectionString) {
            return CommentService.RemoveComment(userId,ids,connectionString);
        }
        public ApiResult<Results, object> GetAllComments(string connectionString) {
            return CommentService.GetAllComments( connectionString);
        }
        public ApiResult<Results, object> GetAllCommentsForUser(int id, string connectionString) {
            return CommentService.GetAllCommentsForUser(id, connectionString);
        }
    }

}
