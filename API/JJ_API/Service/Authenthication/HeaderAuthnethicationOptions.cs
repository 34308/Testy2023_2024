using Microsoft.AspNetCore.Authentication;

namespace JJ_API.Service.Authenthication
{
    public class HeaderAuthenticationOptions : AuthenticationSchemeOptions
    {

        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Role { get; set; }
    }
}
