namespace JJ_API.Models.DTO
{
    public class ResultSignInDto
    {
        public string Token { get; set; }
        public string Avatar { get; set; }

        public ResultSignInDto() { 
        }
        public ResultSignInDto(string token,string avatar)
        {
            Token = token;
            Avatar = avatar;
        }
    }
}
