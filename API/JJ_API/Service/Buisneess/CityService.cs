using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public static class CityService
    {
        public static ApiResult<Results, object> GetAllCitys(string connectionString)
        {
            
            var q_getAllCitys = "SELECT * FROM City ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<City> citys = connection.Query<City>(q_getAllCitys).ToList();
                    if (citys.Count == 0)
                    {
                        return Response(Results.NotFoundAnyCitys);
                    }
                    return Response(Results.OK, citys);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetTouristSpot(int id, string connectionString)
        {
            string q_getCity = "SELECT * FROM City WHERE Id=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    City city = connection.QueryFirstOrDefault<City>(q_getCity, new { id = id });
                    if (city == null)
                    {
                        return Response(Results.NotFoundAnyTouristSpots);
                    }
                    return Response(Results.OK, new List<City> { city });
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddTouristSpots(List<City> input, string connectionString)
        {
            string q_insertTouristSpots = "INSERT INTO City (Name,County,Commune,Voivodeship,BackgroundPhoto,Crest) VALUES (@name,@county,@commune,@voivodeship,@backgroundphoto,@crest)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (City city in input)
                        {
                            int result = connection.Execute(q_insertTouristSpots, new { name = city.Name, county=city.County,commune= city.Commune,voivodeship=city.Voivodeship,backgrundphoto=city.BackgroundPhoto,crest=city.Crest}, transaction);
                            if (result != 1)
                            {
                                transaction.Rollback();
                                return Response(Results.ErrorDuringAddingNewTouristSpots);
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
        public static ApiResult<Results, object> RemoveTouristSpots(List<int> input, string connectionString)
        {
            string q_removeTouristSpots = "Delete FROM City WHERE Id=@id";
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
        public static ApiResult<Results, object> Response(Results results, List<City> register)
        {
            ApiResult<Results, object> result = Response(results);
           
            return new ApiResult<Results, object>(results, result.Message,register);
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
                Results.ErrorDuringAddingCitys => "Error. One of Citys have been not added",
                Results.ErrorDuringRemovingCitys => "Error. One of Citys have been not deleted",
                Results.NotFoundAnyCitys => "Error. No Citys found",
                _ => null
            };
            return new ApiResult<Results, object>(results, message);
        }
    }
}
