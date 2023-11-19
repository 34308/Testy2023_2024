using Dapper;
using JJ_API.Models;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;

namespace JJ_API.Service.Buisneess
{
    public class RouteService
    {
        public static ApiResult<Results, object> GetAllRoutesForUser(int id, string connectionString)
        {
            string q_getAllRoutes = "SELECT * FROM Routes WHERE UserId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<RouteDAO> routes = connection.Query<RouteDAO>(q_getAllRoutes, new { id = id }).ToList();

                    return Response(Results.OK, routes);
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> AddRoutesForUser(RouteDTO routeDTO, string connectionString)
        {
            string q_InsertNewRoute = "INSERT INTO Routes (UserId,RouteName) VALUES (@userid,@routename) ";
            string q_CheckIfRouteAlreadyExist = "Select COUNT(RouteName)  AS RouteNameNumber FROM Routes WHERE UserId=@id AND RouteName=@routename";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (connection.Query<int>(q_CheckIfRouteAlreadyExist, new { id = routeDTO.UserId, routename = routeDTO.RouteName }).FirstOrDefault() != 0)
                    {
                        return Response(Results.RoouteNameAlreadyExist);
                    }
                    int routes = connection.Execute(q_InsertNewRoute, new { userid = routeDTO.UserId, routename = routeDTO.RouteName });
                    if (routes == 1)
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
        
               public static ApiResult<Results, object> RemoveRoutesForUser(int routeId, string connectionString)
        {
            string q_DeleteRoute = "DELETE FROM Routes WHERE Id=@id";
            string q_DeleteRoutesSpots = "DELETE FROM RouteSpots WHERE RouteId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        int deleteRouteResult = connection.Execute(q_DeleteRoute, new { id = routeId }, transaction);
                        int deleteRouteSpotsResult = connection.Execute(q_DeleteRoutesSpots, new { id = routeId }, transaction);

                        if (deleteRouteResult == 1)
                        {
                            transaction.Commit();
                            return Response(Results.OK);
                        }
                        else
                        {
                            transaction.Rollback();
                            return Response(Results.GeneralError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> ChangeNameForRoute(RouteDTO route, string connectionString)
        {

            string q_UpdateNameOfRoute = "UPDATE RouteSpots RouteName=@name WHERE RouteId=@id";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int deleteRouteResult = connection.Execute(q_UpdateNameOfRoute, new { id = route.Id, name = route.RouteName });

                    if (deleteRouteResult == 1)
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
        internal static ApiResult<Results, object> GetSpotsForRouteForUser(int routeId, int userId, string connectionString)
        {
            string q_GetAllRouteSpots = "SELECT rs.RouteId,ts.Id AS TouristSpotId ,ts.Name,rs.[Order] FROM RouteSpots rs JOIN TouristSpot ts ON ts.Id=rs.TouristSpotId WHERE RouteId=@routeid ORDER BY [ORDER]";
            string q_GetPhotos = "SELECT Photo FROM Photo p WHERE p.TouristSpotId=@touristspotid";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<RouteSpotForDragable> SpotsDragable = connection.Query<RouteSpotForDragable>(q_GetAllRouteSpots, new { routeid = routeId }).ToList();


                    foreach (RouteSpotForDragable DragableSpot in SpotsDragable)
                    {
                        DragableSpot.Photo = connection.QueryFirstOrDefault<string>(q_GetPhotos, new { touristspotid = DragableSpot.TouristSpotId });
                    }
                    return Response(Results.OK, SpotsDragable);


                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);
            }
        }
        public static ApiResult<Results, object> GetMapRouteForChoosenRoute(UserLocWithRoute route, bool calculatePath, string connectionString)
        {

            string q_GetAllRouteSpots = "SELECT  ts.Id,ts.Name,ts.Latitude,ts.Longitude  FROM [RouteSpots] rs JOIN TouristSpot ts ON rs.TouristSpotId=ts.Id WHERE RouteId=@id ORDER BY [ORDER]";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    List<RoutePinDao> routePinsDao = connection.Query<RoutePinDao>(q_GetAllRouteSpots, new { id = route.Route.Id }).ToList();
                    List<RoutePin> routePins = new List<RoutePin>();

                    if (routePinsDao.Count > 0)
                    {
                        foreach (RoutePinDao routePin in routePinsDao)
                        {
                            routePins.Add(new RoutePin(routePin));
                        }
                        if (calculatePath)
                        {
                            return Response(Results.OK, RegionService.DistanceCalculator.CratePath(route.UserLocation, routePins));
                        }
                        else
                        {                         
                            return Response(Results.OK,  routePins);
                        }
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
        public static ApiResult<Results, object> Response(Results results, List<RouteSpotForDragable> routes)
        {
            ApiResult<Results, object> result = Response(results);

            return new ApiResult<Results, object>(results, result.Message, routes);
        }
        public static ApiResult<Results, object> Response(Results results, List<RoutePin> routes)
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
                Results.RoouteNameAlreadyExist => "Your Other List Have this name already.",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }


    }
}
