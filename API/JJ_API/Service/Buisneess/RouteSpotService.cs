using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public class RouteSpotService
    {
        public static ApiResult<Results, object> AddRouteSpotToRoute(RouteSpot route, string connectionString)
        {

            string q_InsertRouteSpot = "INSERT INTO RouteSpots (RouteId,TouristSpotId,Order) VALUES (@routeid,@touristspotid,@order)";
            string q_CheckIfExistForRoute = "SELECT COUNT(TouristSpotId) FROM RouteSpots WHERE RouteId=@id and TouristSpotId=@tid";
            string q_getMaxOrder = "SELECT Max([Order]) FROM RouteSpots WHERE RouteId=@id and TouristSpotId=@tid";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (connection.Query<int>(q_CheckIfExistForRoute, new { id = route.RouteId, tid = route.TouristSpotId }).FirstOrDefault() > 0)
                    {
                        return Response(Results.RouteAlreadyAdded);
                    }
                    int maxOrder = connection.Execute(q_getMaxOrder, new { routeid = route.RouteId, touristspotid = route.TouristSpotId });

                    int InsertSpotResult = connection.Execute(q_InsertRouteSpot, new { routeid = route.RouteId, touristspotid = route.TouristSpotId, order = maxOrder+1 });

                    if (InsertSpotResult == 1)
                    {
                        return Response(Results.OK);
                    }
                    else
                    {
                        return Response(Results.GeneralError);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> RemoveSpotFromRoute(RouteSpot route, string connectionString)
        {

            string q_DeleteRouteSpot = "DELETE FROM RouteSpots WHERE RouteId=@routeId AND TouristSpotId=@touristspotid";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int InsertSpotResult = connection.Execute(q_DeleteRouteSpot, new { routeid = route.RouteId, touristspotid = route.TouristSpotId });

                    if (InsertSpotResult == 1)
                    {
                        return Response(Results.OK);
                    }
                    else if (InsertSpotResult == 0)
                    {
                        return Response(Results.NothingWasDeleted);
                    }
                    else
                    {
                        return Response(Results.GeneralError);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> Response(Results results, List<RouteDAO> routes)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, routes);
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
                Results.NothingWasDeleted => "Nothing was found to delete.",
                _ => "ERROR"
            };

            return new ApiResult<Results, object>(results, message);
        }

        internal static ApiResult<Results, object> ChangeRouteSpotOrder(List<RouteSpot> routeSpots, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string q_UpdateListOrder = "UPDATE RouteSpots SET [Order]=@order WHERE TouristSpotId=@touristspotid AND RouteId=@routeid";
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (RouteSpot routeSpot in routeSpots)
                        {
                            int InsertSpotResult = connection.Execute(q_UpdateListOrder, new { order = routeSpot.Order, touristspotid = routeSpot.TouristSpotId, routeid = routeSpot.RouteId }, transaction);
                            if (InsertSpotResult == 0)
                            {
                                transaction.Rollback();
                                return Response(Results.GeneralError);
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
    }
}
