
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JJ_API.Models.DAO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace JJ_API.Service.Authenthication
{
    public class TokenService
    {
        public static string GenerateJwtToken(User user, string secretKey, string issuer, string audience)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.Login),
                new Claim(JwtRegisteredClaimNames.Email,user.Email ),
                new Claim(JwtRegisteredClaimNames.Jti,""+user.Id ),
                new Claim("role",""+user.Role),
             };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(2)),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwtToken);
        }

    }

}
