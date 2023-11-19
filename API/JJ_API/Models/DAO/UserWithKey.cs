using JJ_API.Service.Authenthication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace JJ_API.Models.DAO
{
    public class UserWithKey : User
    {

        public string DecryptedPassword { get; set; }
        public string Key { get; set; }
        public void decryptPassword()
        {
            DecryptedPassword = EncryptionService.DecryptPassword(Password, Key);
        }
    }
}
