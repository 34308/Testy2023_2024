using System.Net;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
using JJ_API.Service.Authenthication;
using JJ_API.Models.DTO;
using JJ_API.Models;

namespace JJ_API.Service.Buisneess
{
    public class RegistrationService
    {
        public static ApiResult<Results, object> RegisterUser(RegisterDto registerModel, string SqlConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    ApiResult<Results, object> api = RegisterUser(registerModel, connection, transaction);
                    if (api.Status == Results.OK)
                    {
                        transaction.Commit();
                    }
                    return api;
                }
            }
        }
        public static ApiResult<Results, object> RegisterUser(RegisterDto registerModel, SqlConnection connection, SqlTransaction transaction)
        {
            Random random = new Random();
            string key;
            int userId = 0;

            string AddNewUserKeyQuery = "INSERT INTO UserKey (userId, [key]) VALUES (@userId, @key)";
            string GetUserIdQuerry = "Select Id FROM [User] WHERE login=@login";
            string AddNewUserQuery = "INSERT INTO [User] (login, email, password,AvatarId) VALUES (@login, @email, @password,@avatar)";
            string CheckIfLoginExist = "SELECT COUNT(Login) as NumberOfLogin FROM [User] WHERE Login=@login";

            string CheckIfEmailExist = "SELECT * FROM [User] WHERE Email=@email";
            int numberOfTheSameLogins = connection.Query<int>(CheckIfLoginExist, new { login = registerModel.Login }, transaction).FirstOrDefault();
            if (numberOfTheSameLogins != 0) { return Response(Results.LoginAlreadyInUse); }
            if (!CheckPassword(registerModel.Password))
            {
                return Response(Results.BadPassword);
            }
            if (!IsValidEmail(registerModel.Email))
            {
                return Response(Results.BadEmail);
            }

            using (SqlCommand command = new SqlCommand(CheckIfEmailExist, connection, transaction))
            {
                command.Parameters.AddWithValue("@email", registerModel.Email);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return Response(Results.EmailAlreadyInUse);
                    }
                }
            };
            key = Convert.ToBase64String(EncryptionService.GenerateRandomKey());
            connection.Execute(AddNewUserQuery, new
            {
                login = registerModel.Login,
                password = EncryptionService.EncryptPassword(registerModel.Password, key),
                email = registerModel.Email,
                avatar=registerModel.AvatarId
            }, transaction);

            userId = connection.Query<int>(GetUserIdQuerry, new { login = registerModel.Login }, transaction).FirstOrDefault();
            connection.Execute(AddNewUserKeyQuery, new { userId, key }, transaction);

            return Response(Results.OK);

        }
 
        public static ApiResult<Results, object> Response(Results results)
        {
            string message = results switch
            {
                Results.OK => "OK.",
                Results.BadPassword => "The password does not meet the requirements.",
                Results.LoginAlreadyInUse => "Login is already in use by another user.",
                Results.EmailAlreadyInUse => "Email is already in use by another user.",
                Results.BadEmail => "Email isn't correct.",
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
        static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        public static ApiResult<Results, object> Response(Results results, RegisterDto register)
        {
            ApiResult<Results, object> result = Response(results);
            var obj = new Dictionary<int, object>();
            obj.Add(0, register);
            return new ApiResult<Results, object>(results, result.Message, obj);
        }
    }

}
