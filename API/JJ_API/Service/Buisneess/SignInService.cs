
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DAO;
using JJ_API.Models.DTO;
using JJ_API.Models;

namespace JJ_API.Service.Buisneess
{
    public class SignInService
    {
        public static ApiResult<Results, object> SignIn(SignInDto signInDto, string sqlConnection,string skey,string issuer,string users)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                connection.Open();
                string CheckLoginQuery = "SELECT Id,Login,Password,[key],Email,AvatarId,[Role] FROM [User] join UserKey ON [User].Id=UserKey.userId WHERE login=@login";
                string InsertTokenIntoDb = "Update UserKey SET jwtToken=@jwtToken WHERE userId=@userId";
                int userId = 0;
                string token = "";
                string avatar = "";

                try
                {
                    UserWithKey userWithKey = connection.QueryFirstOrDefault<UserWithKey>(CheckLoginQuery, new { login = signInDto.Login });
                    if (userWithKey == null)
                    {
                        return Response(Results.NoSuchAccount);
                    }
                    userWithKey.decryptPassword();
                    if (string.Equals(userWithKey.DecryptedPassword, signInDto.Password))
                    {
                        User user = new User { Id = userWithKey.Id, Login = userWithKey.Login, Email = userWithKey.Email,AvatarId=userWithKey.AvatarId,Role=userWithKey.Role };
                        token = TokenService.GenerateJwtToken(user, skey, issuer, users);
                        avatar =((Avatar) AvatarService.GetAvatar(user.AvatarId,sqlConnection).Data).Picture;
                    }
                    else
                    {
                        return Response(Results.BadPassword);
                    }

                    return Response(Results.OK, new ResultSignInDto() { Token=token,Avatar=avatar});
                }
                catch (Exception ex)
                {
                    return Response(Results.GeneralError, ex.Message);
                }
            }
        }

        
        public static ApiResult<Results, object> Response(Results results)
        {
            string message = results switch
            {
                Results.OK => "OK.",
                Results.BadPassword => "The password does not suit this login.",
                Results.NoSuchAccount => "Login dosent much any account.",
                Results.GeneralError => "Error has occured.",
                _ => null
            };

            return new ApiResult<Results, object>(results, message);
        }
        public static bool CheckPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);

        }
       
        
        public static ApiResult<Results, object> Response(Results results, string token)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, token);
        }
        public static ApiResult<Results, object> Response(Results results, ResultSignInDto resultSignIn)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, resultSignIn);
        }
    }
}
