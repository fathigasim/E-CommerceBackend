using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceApplication.DTOs
{
    public class JsonUserDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; } = default!;
        [JsonPropertyName("id")]
        public int Id { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;
    }
}
