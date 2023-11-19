namespace JJ_API.Models.DAO
{
    public class TouristSpot : BasicTouristSpot
    {
        public string Website { get; set; }
        public string Phone { get; set; }

        public string Article { get; set; }
        public List<Image> Images { get; set; }
    }
}
