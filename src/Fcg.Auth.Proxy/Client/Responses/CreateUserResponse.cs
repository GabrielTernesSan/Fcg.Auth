using System.Text.Json.Serialization;

namespace Fcg.Auth.Proxy.User.Client.Responses
{
    public class CreateUserResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
    }
}
