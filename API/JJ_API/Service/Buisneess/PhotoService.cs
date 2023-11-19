using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public static class PhotoService
    {
        public static ApiResult<Results, object> GetPhotosById(int id,string connectionString)
        {
            string q_getPhotoForId = "SELECT * FROM Photo WHERE Id = @id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Image> photos = connection.Query<Image>(q_getPhotoForId, new { id = id }).ToList();
                    return Response(Results.OK, photos);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        
        public static ApiResult<Results, object> AddPhotosForSpot(List<Image> input, string connectionString)
        {
            string q_addPhotoForSpot = "INSERT INTO Photo (TouristSpotId,Photo) VALUES (@spotId,@photo)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (Image photo in input)
                        {
                            int result = connection.Execute(q_addPhotoForSpot, new { spotId = photo.TouristSpotId, photo.Photo },transaction);
                            if (result == 0)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringAddingNewPhotos);
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
        public static ApiResult<Results, object> RemovePhotosForSpot(List<int> Ids, string connectionString)
        {
            string q_removePhotoForSpot = "DELETE FROM Photo WHERE Id=@id)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (int id in Ids)
                        {
                            int result = connection.Execute(q_removePhotoForSpot, new { id = id },transaction);
                            if (result == 0)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringRemovingPhotos);
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
        public static ApiResult<Results, object> GetAllPhotos( string connectionString)
        {
            string q_getPhotos = "SELECT * FROM Photo";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Image> photos = connection.Query<Image>(q_getPhotos).ToList();
                    return Response(Results.OK, photos);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetPhotoForTouristSpot(int touristSpotId,string connectionString)
        {
            string q_getPhotos = "SELECT * FROM Photo Where TouristSpotId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Image photos = connection.QueryFirstOrDefault<Image>(q_getPhotos, new { id = touristSpotId });
                    return Response(Results.OK,new List<Image> { photos });
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetPhotosForTouristSpot(int touristSpotId, string connectionString)
        {
            string q_getPhotos = "SELECT * FROM Photo Where TouristSpotId = @id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Image> photos = connection.Query<Image>(q_getPhotos, new { id = touristSpotId }).ToList();
                    return Response(Results.OK, photos);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<Image> images)
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
