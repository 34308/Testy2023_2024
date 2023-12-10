namespace JJ_API.Models.DAO
{
    public class TouristSpot : BasicTouristSpot
    {
        public string Website { get; set; }
        public string Phone { get; set; }

        public string Article { get; set; }
        public List<Image> Images { get; set; }
        public TouristSpot() { }
        
            public TouristSpot(string name, string openTime, string closeTime,
            float score, Image MainPhoto, Address address, string description, float latitude,
            float longitude, string website, string phone, string article,List<Image> images) {
            this.Score = score;
            this.Name = name;
            this.OpenTime = openTime;
            this.CloseTime = closeTime;
            this.Score = score;
            this.MainPhoto = MainPhoto;
            this.Address = address;
            this.Description = description;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Website = website;
            this.Images = images;
            this.Article = article;
            this.Phone = phone;

        }

    }
}
