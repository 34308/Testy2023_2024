using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public static class TouristSpotService
    {

        public static ApiResult<Results, object> GetAllTouristSpots(string connectionString)
        {
            string q_getTouristSpots = "SELECT * FROM TouristSpot ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<TouristSpot> touristSpots = connection.Query<TouristSpot>(q_getTouristSpots).ToList();

                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }


        public static ApiResult<Results, object> GetAllTouristSpotsForCity(string city, string connectionString)
        {
            string q_getTouristSpotsForCity = "SELECT ts.Id,Name,AddressId,Website,Phone,Latitude,Longitude,OpenTime,CloseTime,Score,Description,Article FROM (TouristSpot ts Join [Address] as ad ON ts.AddressId=ad.Id) WHERE City=@city";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<TouristSpot> touristSpots = connection.Query<TouristSpot>(q_getTouristSpotsForCity, new { city = city }).ToList();


                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }

        public static ApiResult<Results, object> GetTouristSpot(int id, string connectionString, bool allPhotos = false)
        {
            string q_getTouristSpots = "SELECT * FROM TouristSpot WHERE Id=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    TouristSpot touristSpot = connection.QueryFirstOrDefault<TouristSpot>(q_getTouristSpots, new { id = id });
                    List<Image> photo = new List<Image>();
                    if (!allPhotos)
                    {
                        photo = PhotoService.GetPhotoForTouristSpot(id, connectionString).Data as List<Image>;
                        touristSpot.MainPhoto = photo[0];
                    }
                    else
                    {
                        photo = PhotoService.GetPhotosForTouristSpot(id, connectionString).Data as List<Image>;
                        touristSpot.MainPhoto = photo[0];
                        touristSpot.Images = photo;
                    }

                    if (touristSpot == null)
                    {
                        return Response(Results.NotFoundAnyTouristSpots);
                    }
                    return Response(Results.OK, touristSpot);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddTouristSpots(TouristSpot input, string connectionString)
        {
            string q_insertTouristSpots = "INSERT INTO TouristSpot (Name,[AddressId],Website,Phone,Latitude,Longitude,OpenTime,CloseTime) OUTPUT Inserted.Id " +
                "VALUES (@name,@address,@website,@phone,@latitude,@longitude,@opentime,@closetime)";
            string q_addAdress = "INSERT INTO [Address] (Street,Number,City,PostalCode,Country,TouristSpotId) Values (@street,@number,@city,@postalcode,@country,@tsid)";
            string q_addPhoto = "INSERT INTO [Photo] (TouristSpotId,Photo) Values (@tsid,@photo)";

            int newId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {

                        newId = connection.QueryFirstOrDefault<int>(q_insertTouristSpots, new
                        {
                            name = input.Name,
                            address = 0,
                            website = input.Website,
                            phone = input.Phone,
                            latitude = input.Latitude,
                            longitude = input.Longitude,
                            opentime = ""+input.OpenTime,
                            closetime = "" + input.CloseTime
                        }, transaction) ;
                        if (newId <= 0)
                        {
                            transaction.Rollback();
                            return Response(Results.ErrorDuringAddingNewTouristSpots);
                        }
                        connection.Execute(q_addAdress, new
                        {
                            street = input.Address.Street,
                            number = input.Address.Number,
                            city = input.Address.City,
                            postalcode = input.Address.PostalCode,
                            country = input.Address.Country,
                            tsid = newId,
                        }, transaction);
                        foreach (Image image in input.Images)
                        {
                            connection.Execute(q_addPhoto, new { tsid = newId, photo = image.Photo }, transaction);
                        }

                        transaction.Commit();
                        return Response(Results.OK, newId);
                    }

                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> RemoveTouristSpots(List<int> input, string connectionString)
        {
            string q_removeTouristSpots = "Delete FROM TouristSpot WHERE Id=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (int id in input)
                        {
                            int result = connection.Execute(q_removeTouristSpots, new { id = id }, transaction);
                            if (result != 1)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringRemovingTouristSpots);
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
        public static ApiResult<Results, object> GetVisitedSpots(int id, string connectionString)
        {
            string q_getVisitedTouristSpot = "SELECT * FROM VisitedSpot WHERE UserId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<VisitedTouristSpot> visitedTouristSpots = connection.Query<VisitedTouristSpot>(q_getVisitedTouristSpot, new { id = id }).ToList();

                    return Response(Results.OK, visitedTouristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<TouristSpot> touristSpots)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, touristSpots);
        }
        public static ApiResult<Results, object> Response(Results results, int id)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, id);
        }
        public static ApiResult<Results, object> Response(Results results, List<VisitedTouristSpot> touristSpots)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, touristSpots);
        }
        public static ApiResult<Results, object> Response(Results results, TouristSpot touristSpots)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, touristSpots);
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
                Results.ErrorDuringAddingNewTouristSpots => "Error. One of Tourist Spots have been not added",
                Results.ErrorDuringRemovingTouristSpots => "Error. One of Tourist Spots have been not deleted",
                Results.NotFoundAnyTouristSpots => "Error. No tourist spots found",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }

        internal static object? GetTouristSpotsForCity(string city, int id, string connectionString)
        {
            throw new NotImplementedException();
        }
    }

}
