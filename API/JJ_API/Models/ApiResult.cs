using System.Text.Json.Serialization;

namespace JJ_API.Models
{
    [Serializable]
    public class ApiResult<T1, T2>
    {
        
       
        public T1 Status { get; set; }
      
        public object Data { get; }

     
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
