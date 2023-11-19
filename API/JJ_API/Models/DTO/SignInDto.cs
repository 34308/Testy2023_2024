using System.Text.Json.Serialization;

namespace JJ_API.Models.DTO
{
    public class SignInDto
    {
        [JsonPropertyName("login")]

        public string Login { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
