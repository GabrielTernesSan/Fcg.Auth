using System.Text.Json.Serialization;

namespace Fcg.Auth.Application.Responses
{
    public class ExternalResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
    }
}
