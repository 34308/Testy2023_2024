namespace JJ_API.Models.DAO
{
    public class BasicTouristSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public float Score { get; set; }
        public Image MainPhoto { get; set; }
        public Address Address { get; set; }
        public string Description { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
