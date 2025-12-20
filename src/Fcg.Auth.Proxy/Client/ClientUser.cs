using Fcg.Auth.Common;
using Fcg.Auth.Proxy.User.Client.Interface;
using Fcg.Auth.Proxy.User.Client.Responses;
using Fcg.Auth.Proxy.User.Configurations;
using Microsoft.Extensions.Options;
using System.Text;

namespace Fcg.Auth.Proxy.User.Client
{
    public class ClientUser : IClientUser
    {
        private readonly UserConfiguration _userConfiguration;
        private readonly HttpClient _httpClient;

        public ClientUser(IOptions<UserConfiguration> userConfiguration, HttpClient httpClient)
        {
            _userConfiguration = userConfiguration.Value;
            _httpClient = httpClient;
        }

        public async Task<Response<CreateUserResponse>> CreateUserAsync(Guid id)
        {
            var response = new Response<CreateUserResponse?>();

            var url = $"{_userConfiguration.Url}/users";

            var payload = new
            {
                id
            };

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(payload),
                        Encoding.UTF8,
                        "application/json"
                    )
                };

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var status = (int)httpResponseMessage.StatusCode;
                    var reason = httpResponseMessage.ReasonPhrase ?? "Sem motivo informado";
                    var body = await httpResponseMessage.Content.ReadAsStringAsync();

                    response.AddError($"Erro ao enviar dados ({status} {reason}). Detalhes: {body}");
                    return response;
                }

                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                response.Result = System.Text.Json.JsonSerializer.Deserialize<CreateUserResponse>(json);
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao enviar dados ({ex.Message})");
            }

            return response;
        }
    }
}
