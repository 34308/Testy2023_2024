using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public class AddressService
    {
        public static ApiResult<Results, object> GetAdressForId(int id,string connectionString)
        {
            string q_getAddress = "SELECT * FROM Address WHERE TouristSpotId=@id";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Address address = connection.Query<Address>(q_getAddress, new { id=id}).FirstOrDefault();

                    return Response(Results.OK, address);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddAdressForId(Address address, string connectionString)
        {
            string q_addAddress = "INSERT INTO Address (Street,Number,City,PostalCode,Country,TouristSpotId) VALUES (@street,@number,@city,@postalcode,@country,@touristspotid)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int result = connection.Execute(q_addAddress,new {street=address.Street,number=address.Number,city=address.City,postalcode=address.PostalCode,country=address.Country, touristspotid=address.TouristSpotId });
                    if (result != 1)
                    {
                        return Response(Results.GeneralError);
                    }
                    return Response(Results.OK);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> DeleteAdressForId(int id, string connectionString)
        {
            string q_deleteAddress = "DELETE FROM Address WHERE Id=@id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int result = connection.Execute(q_deleteAddress, new { id=id });
                    if (result != 1)
                    {
                        return Response(Results.GeneralError);
                    }
                    return Response(Results.OK);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, Address address )
        {
            ApiResult<Results, object> result = Response(results);
           
            return new ApiResult<Results, object>(results, result.Message, address);
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
               
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }
    }
}
