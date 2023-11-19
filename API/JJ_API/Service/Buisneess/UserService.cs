using Dapper;
using JJ_API.Models;
using JJ_API.Models.DTO;
using JJ_API.Service.Authenthication;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace JJ_API.Service.Buisneess
{
    public static class UserService
    {

        public static ApiResult<Results, object> UpdateUser(int userId, RegisterDto registerModel, string SqlConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    ApiResult<Results, object> api = UpdateUser(userId, registerModel, connection, transaction);
                    if (api.Status == Results.OK)
                    {
                        transaction.Commit();
                    }
                    return api;
                }
            }

        }
        public static ApiResult<Results, object> UpdateUser(int userId, RegisterDto registerModel, SqlConnection connection, SqlTransaction transaction)
        {
            Random random = new Random();
            string key;

            string AddNewUserKeyQuery = "UPDATE UserKey key=@key) WHERE UserId=@id)";

            string q_UpdateUser = "UPDATE [User] SET Login=@login, Password=@cryptedPassword, Email=@email";
            string q_CheckIfLoginExist = "SELECT Login FROM [User] WHERE Id=@id";
            string q_CheckIfEmailExist = "SELECT Email FROM [User] WHERE Id=@id";

            if (!CheckPassword(registerModel.Password))
            {
                return Response(Results.BadPassword);
            }
            if (!IsValidEmail(registerModel.Email))
            {
                return Response(Results.BadEmail);
            }
            key = Convert.ToBase64String(EncryptionService.GenerateRandomKey());
            string oldLogin = connection.QueryFirstOrDefault<string>(q_CheckIfLoginExist, new { id = userId });
            if (oldLogin != registerModel.Login)
            {
                int loginNumber = connection.QueryFirstOrDefault<int>("SELECT COUNT(Id) FROM [USER] Where Login=@login", new { login = registerModel.Login });
                if (loginNumber > 0)
                {
                    return (Response(Results.LoginAlreadyInUse));
                }
            }
            string oldEmail = connection.QueryFirstOrDefault<string>(q_CheckIfEmailExist, new { id = userId });
            if (oldEmail != registerModel.Email)
            {
                int EmailNumber = connection.QueryFirstOrDefault<int>("SELECT COUNT(Id) Where Email=@email", new { email = registerModel.Email });
                if (EmailNumber > 0)
                {
                    return (Response(Results.EmailAlreadyInUse));
                }
            }
            int result = connection.Execute(q_UpdateUser, new
            {
                login = registerModel.Login,
                cryptedPassword = EncryptionService.EncryptPassword(registerModel.Password, key),
                email = registerModel.Email
            }, transaction);
            if (result == 1)
            {
                connection.Execute(AddNewUserKeyQuery, new { id = userId, key = key }, transaction);

                return Response(Results.OK);
            }
            else
            {
                return Response(Results.UnexpectedError);
            }

        }

        public static ApiResult<Results, object> ChangeAvatarForExistingOne(int userId, int avatarId, string SqlConnectionString)
        {
            try
            {
                string q_UpdateAvatar = "UPDATE [User] SET AvatarId=@avatarid WHERE Id=@id";
                using (SqlConnection connection = new SqlConnection(SqlConnectionString))
                {
                    connection.Open();
                    int result = connection.Execute(q_UpdateAvatar, new { avatarId = avatarId, id = userId });
                    if (result != 1)
                    {
                        return Response(Results.UnexpectedError);

                    }
                    else
                    {
                        return Response(Results.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Response(Results.GeneralError, ex.Message);

            }

        }
        public static ApiResult<Results, object> ChangeAvatarForNewOne(int userId, string avatarString, string SqlConnectionString)
        {
            string q_UpdateAvatar = "UPDATE [User] AvatarId=@avatarid WHERE Id=@id";
            string q_InsertNewAvatar = "INSERT INTO Avatar (Picture) VALUES (@picture)";
            string q_GetLatestAvatar = "SELECT TOP(1) Id FROM Avatar ORDER BY Id DESC";

            using (SqlConnection connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                int resultOfInsert = connection.Execute(q_InsertNewAvatar, new { picture = avatarString });
                if (resultOfInsert != 1)
                {
                    return Response(Results.AvatarNotInserted);

                }
                int avatarId = connection.Execute(q_GetLatestAvatar, new { picture = avatarString });

                int result = connection.Execute(q_UpdateAvatar, new { avatarId = avatarId, id = userId });
                if (result != 1)
                {
                    return Response(Results.UnexpectedError);
                }
                else
                {
                    return Response(Results.OK);
                }
            }
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
                Results.AvatarNotInserted => "Avatar has not been inserted properly",
                Results.GeneralError => "General Error",
                Results.IncorrectPasswordForUser=>"Niepoprawne hasło",
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
            return new ApiResult<Results, object>(results, result.Message, register);
        }
        public static ApiResult<Results, object> Response(Results results, string register)
        {
            ApiResult<Results, object> result = Response(results);
            return new ApiResult<Results, object>(results, result.Message, register);
        }

        internal static ApiResult<Results, object> ChangeUserPassword(int userId, ChangePasswordDto input, string connectionString)
        {
            string key;
            string AddNewUserKeyQuery = "UPDATE UserKey SET [key]=@key WHERE [UserId]=@id";

            string q_UpdateUser = "UPDATE [User] SET [Password]=@cryptedPassword WHERE [Id]=@uid";

            if (!CheckPassword(input.NewPassword))
            {
                return Response(Results.IncorrectPasswordForUser);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                if (!CheckPassword(input.OldPassword, userId, connection))
                {
                    return Response(Results.BadPassword);
                }
                SqlTransaction transaction = connection.BeginTransaction();
                key = Convert.ToBase64String(EncryptionService.GenerateRandomKey());

                int result = connection.Execute(q_UpdateUser, new
                {
                    cryptedPassword = EncryptionService.EncryptPassword(input.NewPassword, key),
                    uid = userId,
                }, transaction);
                if (result == 1)
                {
                    int keyResult = connection.Execute(AddNewUserKeyQuery, new { id = userId, key = key }, transaction);
                    if (keyResult == 1)
                    {
                        transaction.Commit();
                        return Response(Results.OK);
                    }
                    else
                    {
                        transaction.Rollback();
                        return Response(Results.UnexpectedError);
                    }
                }
                else
                {
                    transaction.Rollback();
                    return Response(Results.UnexpectedError);
                }
            }

        }

        internal static ApiResult<Results, object> UpdateEmail(int userId, ChangeEmailDto input, string connectionString)
        {
            if (!IsValidEmail(input.Email))
            {
                return Response(Results.BadEmail);
            }
            string q_CheckIfEmailExist = "SELECT Email FROM [User] WHERE Id=@id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (!CheckPassword(input.Password, userId, connection))
                {
                    return Response(Results.IncorrectPasswordForUser);
                }
                string oldLogin = connection.QueryFirstOrDefault<string>(q_CheckIfEmailExist, new { id = userId });
                if (oldLogin != input.Email)
                {
                    int emailNumber = connection.QueryFirstOrDefault<int>("SELECT COUNT(Id) FROM [USER] Where Email=@email", new { email = input.Email });
                    if (emailNumber > 0)
                    {
                        return (Response(Results.EmailAlreadyInUse));
                    }
                }
                string q_UpdateEmailQuery = "UPDATE [USER] SET Email=@email WHERE Id=@uid";
               
                int result = connection.Execute(q_UpdateEmailQuery, new
                {
                    email = input.Email,
                    uid = userId,
                });
                if (result == 1)
                {
                    return Response(Results.OK);
                }
                else
                {
                    return Response(Results.UnexpectedError);
                }
            }
        }
        public static bool CheckPassword(string password,int userId,SqlConnection connection)
        {
            string q_CheckUserPassword = "SELECT Password FROM [USER] WHERE Id=@uid";
            string q_GetUserKey = "SELECT [Key] FROM [UserKey] WHERE userId=@uid";

            string cryptedPassword = connection.QueryFirstOrDefault<string>(q_CheckUserPassword, new {uid=userId });
            string key = connection.QueryFirstOrDefault<string>(q_GetUserKey, new {uid=userId });
            if(EncryptionService.DecryptPassword(cryptedPassword, key) == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static ApiResult<Results, object> UpdateLogin(int userId, ChangeLoginDto input, string connectionString)
        {
            string q_CheckIfLoginExist = "SELECT Login FROM [User] WHERE Id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (!CheckPassword(input.Password, userId, connection))
                {
                    return Response(Results.IncorrectPasswordForUser);
                }
                string oldLogin = connection.QueryFirstOrDefault<string>(q_CheckIfLoginExist, new { id = userId });
                if (oldLogin != input.Login)
                {
                    int loginNumber = connection.QueryFirstOrDefault<int>("SELECT COUNT(Id) FROM [USER] Where Login=@login", new { login = input.Login });
                    if (loginNumber > 0)
                    {
                        return (Response(Results.LoginAlreadyInUse));
                    }
                }
                string q_UpdateLoginQuery = "UPDATE [USER] SET Login=@login WHERE Id=@uid";
                int result = connection.Execute(q_UpdateLoginQuery, new
                {
                    login = input.Login,
                    uid = userId,
                });
                if (result == 1)
                {
                    return Response(Results.OK);
                }
                else
                {
                    return Response(Results.UnexpectedError);
                }
            }
        }
    }
}

