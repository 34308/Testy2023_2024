using Newtonsoft.Json;

namespace JJ_API.Models.DAO
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int Number { get; set; }
        public int TouristSpotId { get; set; }
    }
}