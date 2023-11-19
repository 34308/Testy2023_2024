using JJ_API.Models;
using Dapper;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JJ_API.Service.Buisneess
{
    public static class BasicTouristSpotService
    {
        public static ApiResult<Results, object> GetAllBasicTouristSpots(string connectionString)
        {
            string q_getBasicTouristSpot = "SELECT Id,Name,OpenTime,CloseTime,Score,Latitude,Longitude FROM TouristSpot";
            string q_getPhotoForId = "SELECT * FROM Photo WHERE TouristSpotId = @id";
            string q_getEmergencyPhoto = "SELECT Id,TouristSpotId,Photo FROM Photo WHERE Id = 1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<BasicTouristSpot> touristSpots = connection.Query<BasicTouristSpot>(q_getBasicTouristSpot).ToList();

                    foreach (BasicTouristSpot basic in touristSpots)
                    {
                        var result = AddressService.GetAdressForId(basic.Id, connectionString);
                        if (result.Status != 0)
                        {
                            return Response(Results.GeneralError, result.Message);
                        }
                        basic.Address = (Address)result.Data;
                        basic.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getPhotoForId, new { id = basic.Id });
                        if (basic.MainPhoto == null)
                        {
                            basic.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getEmergencyPhoto);
                        }
                    }

                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetAllVisitedTouristSpotsForUser(int userId, string connectionString)
        {
            string q_getTouristSpotsForCity = "SELECT ts.Id,Name,AddressId,Website,Phone,Latitude,Longitude,OpenTime,CloseTime,Score,Description,Article " +
                "FROM (TouristSpot ts Join [Address] as ad ON ts.AddressId=ad.Id) JOIN VisitedSpot as vs ON vs.TouristSpotId=ts.Id " +
                "WHERE UserId=@userid";
            string q_getPhotoForId = "SELECT * FROM Photo WHERE TouristSpotId = @id";
            string q_getEmergencyPhoto = "SELECT Id,TouristSpotId,Photo FROM Photo WHERE Id = 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<BasicTouristSpot> touristSpots = connection.Query<BasicTouristSpot>(q_getTouristSpotsForCity, new { userid = userId }).ToList();
                    foreach (var touristSpot in touristSpots)
                    {
                        var result = AddressService.GetAdressForId(touristSpot.Id, connectionString);
                        if (result.Status != 0)
                        {
                            return Response(Results.GeneralError, result.Message);
                        }
                        touristSpot.Address = (Address)result.Data;
                        touristSpot.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getPhotoForId, new { id = touristSpot.Id });
                        if (touristSpot.MainPhoto == null)
                        {
                            touristSpot.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getEmergencyPhoto);
                        }
                    }
                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetTouristSpotsForMapPins(PinsIds TouristSpotsIds, string connectionString)
        {
            string q_getBasicTouristSpot = "SELECT Id,Name FROM TouristSpot WHERE Id IN @ids";
            string q_getPhotoForId = "SELECT * FROM Photo WHERE TouristSpotId = @id";
            string q_getEmergencyPhoto = "SELECT Id,TouristSpotId,Photo FROM Photo WHERE Id = 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<TouristSpotPinDAO> touristSpots = connection.Query<TouristSpotPinDAO>(q_getBasicTouristSpot, new { ids = TouristSpotsIds.Ids }).ToList();
                    foreach (TouristSpotPinDAO touristSpot in touristSpots)
                    {
                        var result = AddressService.GetAdressForId(touristSpot.Id, connectionString);
                        if (result.Status != 0)
                        {
                            return Response(Results.GeneralError, result.Message);
                        }

                        touristSpot.Photo = connection.QueryFirstOrDefault<Image>(q_getPhotoForId, new { id = touristSpot.Id });
                        if (touristSpot.Photo == null)
                        {
                            touristSpot.Photo = connection.QueryFirstOrDefault<Image>(q_getEmergencyPhoto);
                        }
                    }
                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<BasicTouristSpot> basicTouristSpots)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, basicTouristSpots);
        }
        public static ApiResult<Results, object> Response(Results results, List<TouristSpotPinDAO> basicTouristSpots)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, basicTouristSpots);
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
                Results.NothingWasDeleted => "No Record from Visited Spots was deleted",
                Results.MoreThanOneRecordHasBeenDeleted => "More Than one record from visited spots has benn deleted. when expected only one.",
                Results.VisitedSpotAlreadyAdded => "Visited Spot was already added.",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }
        public static ApiResult<Results, object> VisitedTouristSpotsForUser(int userId, string connectionString)
        {
            string q_getTouristSpotsForUser = "SELECT ts.Id,ts.Name,OpenTime,CloseTime,Score,Description,Longitude,Latitude " +
               "FROM (TouristSpot ts JOIN Address ad ON ts.AddressId=ad.id) WHERE ts.Id=@id";
            string q_IdsForTouristSpots = "SELECT TouristSpotId FROM VisitedSpot WHERE UserId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<int> ids = connection.Query<int>(q_IdsForTouristSpots, new { id = userId }).ToList();
                    List<BasicTouristSpot> touristSpots = new List<BasicTouristSpot>();
                    foreach (int id in ids)
                    {
                        touristSpots.AddRange(connection.Query<BasicTouristSpot>(q_getTouristSpotsForUser, new { id = id }).ToList());

                    }
                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> RemoveVisitedTouristSpotForUser(int userId, int SpotId, string connectionString)
        {

            string q_RemoveSpotFromUser = "DELETE FROM VisitedSpot WHERE UserId=@id AND TouristSpotId=@spotid";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int result = connection.Execute(q_RemoveSpotFromUser, new { id = userId, spotid = SpotId });
                    if (result == 0)
                    {
                        return Response(Results.NothingWasDeleted);
                    }
                    else if (result > 1)
                    {
                        return Response(Results.MoreThanOneRecordHasBeenDeleted);
                    }
                    else
                    {
                        return Response(Results.OK);

                    }

                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddVisitedTouristSpotForUser(int userId, int SpotId, string connectionString)
        {
            string q_CheckIfExist = "SELECT COUNT(Id) FROM VisitedSpot WHERE TouristSpotId=@tsi AND UserId=@userid";

            string q_AddvisitedSpot = "INSERT INTO VisitedSpot (TouristSpotId,UserId) VALUES (@touristspot,@userid)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int resultOfCheck = connection.QueryFirstOrDefault<int>(q_CheckIfExist, new { tsi = SpotId, userid = userId });
                    if (resultOfCheck != 0)
                    {
                        return Response(Results.VisitedSpotAlreadyAdded);
                    }
                    int result = connection.Execute(q_AddvisitedSpot, new { touristspot = SpotId, userid = userId });
                    if (result != 1)
                    {
                        return Response(Results.UnexpectedError);
                    }
                    return Response(Results.OK);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetBasicTouristSpotsForCity(string city, string connectionString)
        {
            string q_getTouristSpotsForCity = "SELECT ts.Id,ts.Name,OpenTime,CloseTime,Score,Description,Longitude,Latitude " +
                "FROM (TouristSpot ts JOIN Address ad ON ts.AddressId=ad.id) WHERE ad.City=@city";
            string q_getPhotoForId = "SELECT * FROM Photo WHERE TouristSpotId = @id";
            string q_getEmergencyPhoto = "SELECT Id,TouristSpotId,Photo FROM Photo WHERE Id = 1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<BasicTouristSpot> touristSpots = connection.Query<BasicTouristSpot>(q_getTouristSpotsForCity, new { city = city }).ToList();

                    foreach (BasicTouristSpot basic in touristSpots)
                    {
                        var result = AddressService.GetAdressForId(basic.Id, connectionString);
                        if (result.Status != 0)
                        {
                            return Response(Results.GeneralError, result.Message);
                        }
                        basic.Address = (Address)result.Data;
                        basic.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getPhotoForId, new { id = basic.Id });
                        if (basic.MainPhoto == null)
                        {
                            basic.MainPhoto = connection.QueryFirstOrDefault<Image>(q_getEmergencyPhoto);
                        }
                    }

                    return Response(Results.OK, touristSpots);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
    }
}
