
using System.IdentityModel.Tokens.Jwt;

using System.Text;
using System.Text.Encodings.Web;
using JJ_API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JJ_API.Service.Authenthication
{
    public class AuthorizeUserIdAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("jti");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var routeDataUserId = context.RouteData.Values["userId"];
            if (routeDataUserId == null || !int.TryParse(routeDataUserId.ToString(), out int routeUserId) || userId != routeUserId)
            {
                var isAdminClaim = context.HttpContext.User.FindFirst("role");
                if (isAdminClaim == null || !int.TryParse(isAdminClaim.Value, out int isAdmin) || isAdmin != (int)Enums.UserLevel.Admin)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }
    public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
    {
        public IServiceProvider ServiceProvider { get; set; }
        private readonly IOptionsMonitor<HeaderAuthenticationOptions> _options;


        public HeaderAuthenticationHandler(IOptionsMonitor<HeaderAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider serviceProvider)
            : base(options, logger, encoder, clock)
        {
            ServiceProvider = serviceProvider;
            _options = options;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headers = Request.Headers;

            if (!headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
            }

            var token = authorizationHeader.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Token is null"));
            }

            var options = _options.Get(Scheme.Name);
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience
                };

                var principal = jwtTokenHandler.ValidateToken(token, validationParameters, out _);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(AuthenticateResult.Fail($"Failed to validate JWT token: {ex.Message}"));
            }
        }
    }

}
