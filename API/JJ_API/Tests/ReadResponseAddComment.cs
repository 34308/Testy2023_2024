using JJ_API.Models.DTO;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JJ_API.Tests
{
    public class ReadResponseAddComment
    {
        public int Status { get; set; }
        public int Data { get; set; }
        public string Message { get; set; }
    }
}
