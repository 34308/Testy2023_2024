using JJ_API.Models.DTO;

namespace JJ_API.Models
{
    public class ReadResponseClas
    {
        public int Status { get; set; }
        public ResultSignInDto Data { get; set; }
        public string Message { get; set; }

    }
}
