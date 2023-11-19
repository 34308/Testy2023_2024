using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public class AvatarService
    {
        public static ApiResult<Results, object> GetAvatar(int id, string connectionString)
        {
            string q_getAvatarForUser = "SELECT * FROM Avatar WHERE Id = @id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Avatar photos = connection.QueryFirstOrDefault<Avatar>(q_getAvatarForUser, new { id = id });
                    if (photos == null)
                    {
                        photos=new Avatar();
                    }
                    return Response(Results.OK, photos);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetAvatarForUser(int id, string connectionString)
        {
            string q_UserAvatarId = "SELECT AvatarId FROM [User] WHERE Id = @id";
            string q_getAvattarForId = "SELECT * FROM Avatar WHERE Id = @id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int avatarid = connection.Query<int>(q_UserAvatarId, new { id = id }).FirstOrDefault();

                    Avatar photos = connection.QueryFirstOrDefault<Avatar>(q_getAvattarForId, new { id = avatarid });
                    if (photos == null)
                    {
                        photos = new Avatar();
                    }
                    return Response(Results.OK, photos);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddNewAvatar(Avatar input, string connectionString)
        {
            string q_addPhotoForSpot = "INSERT INTO Avatar (Picture) VALUES (@picture)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {

                        int result = connection.Execute(q_addPhotoForSpot, new { picture = input.Picture }, transaction);
                        if (result == 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewPhotos);
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
        public static ApiResult<Results, object> GetAllBasicAvatars(string connectionString)
        {
            int numberOfAvatar = 10;
            string q_getbasicAvatars = "SELECT TOP "+ numberOfAvatar + " * FROM Avatar";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Avatar> avatar = connection.Query<Avatar>(q_getbasicAvatars).ToList();
                    return Response(Results.OK, avatar);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<Avatar> images)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, images);
        }
        public static ApiResult<Results, object> Response(Results results, Avatar images)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, images);
        }
        public static ApiResult<Results, object> Response(Results results, string token)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, token);
        }
        public static ApiResult<Results, object> Response(Results results)
        {
            string message = results switch
            {
                Results.OK => "OK.",
                Results.GeneralError => "Error has occured.",
                Results.ErrorDuringAddingNewPhotos => "Error. One of photos have been not added",
                Results.ErrorDuringRemovingPhotos => "Error. One of photos have been not deleted",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }
    }
}
