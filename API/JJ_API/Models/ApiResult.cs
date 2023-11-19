using System.Text.Json.Serialization;

namespace JJ_API.Models
{
    [Serializable]
    public class ApiResult<T1, T2>
    {
        
        [JsonPropertyName("status")]
        public T1 Status { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public ApiResult(T1 value, string message, T2 data)
        {
            Status = value;
            Message = message;
            Data = data;
        }
        public ApiResult(T1 value, string message, string data)
        {
            Status = value;
            Message = message;
            Data = data;
        }
        public ApiResult(T1 value, T2 data)
        {
            Status = value;
            Data = data;
        }

        public ApiResult(T1 value, string message)
        {
            Status = value;
            Message = message;
        }

        public ApiResult(T1 value)
        {
            Status = value;
        }
    }
}
