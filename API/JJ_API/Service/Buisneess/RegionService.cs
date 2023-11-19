using JJ_API.Models.DTO;

namespace JJ_API.Service.Buisneess
{
    public class RegionService
    {
        public class DistanceCalculator
        {
            public static double CalculateDistance(CoordinatesDto point1, CoordinatesDto point2)
            {
                double earthRadius = 6371;

                double lat1 = Math.PI * point1.Latitude / 180;
                double lat2 = Math.PI * point2.Latitude / 180;

                double deltaLat = Math.PI * (point2.Latitude - point1.Latitude) / 180;
                double deltaLon = Math.PI * (point2.Longitude - point1.Longitude) / 180;

                double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                           Math.Cos(lat1) * Math.Cos(lat2) *
                           Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                return earthRadius * c;
            }

            public static double CalculateTotalDistance(List<CoordinatesDto> points)
            {
                double totalDistance = 0;

                for (int i = 0; i < points.Count - 1; i++)
                {
                    totalDistance += CalculateDistance(points[i], points[i + 1]);
                }

                return totalDistance;
            }
            public static RoutePin FindClosestPoint(CoordinatesDto startingPosition, List<RoutePin> allPositionsWithoutStartingPos)
            {
                double minDistance = double.MaxValue;
                RoutePin closestPoint = null;

                foreach (var position in allPositionsWithoutStartingPos)
                {
                    double distance = CalculateDistance(startingPosition, position.Coordinates);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = position;
                    }
                }

                return closestPoint;
            }
            public static List<RoutePin> CratePath(CoordinatesDto userPosition,List<RoutePin> coordinatesDtos)
            {
                List<RoutePin> SortedList = new List<RoutePin>();
                SortedList.Add(FindClosestPoint(userPosition, coordinatesDtos));
                coordinatesDtos.Remove(SortedList[0]);
                for(int i=0;i<coordinatesDtos.Count-1;i++)
                {
                    SortedList.Add(FindClosestPoint(SortedList[i].Coordinates, coordinatesDtos));
                    coordinatesDtos.Remove(SortedList[SortedList.Count-1]);
                }
                return SortedList;
            }
        }
    }
}
