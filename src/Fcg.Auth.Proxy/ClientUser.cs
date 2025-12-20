using Fcg.Auth.Common;
using Fcg.Auth.Proxy.User.Interface;
using Fcg.Auth.Proxy.User.Responses;
using System.Net.Http;
using System.Text;

namespace Fcg.Auth.Proxy.User
{
    public class ClientUser : IClientUser
    {
        private readonly UserConfiguration _userConfiguration;
        private readonly HttpClient _httpClient;

        public ClientUser(UserConfiguration userConfiguration, HttpClient httpClient)
        {
            _userConfiguration = userConfiguration;
            _httpClient = httpClient;
        }

        public async Task<Response<CreateUserResponse>> CreateUserAsync(Guid id)
        {
            var response = new Response<CreateUserResponse?>();

            var url = _userConfiguration.Url;

            var request = $@"";

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(request, Encoding.UTF8, "text/xml")
                };

                httpRequestMessage.Headers.Add("SOAPAction");

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var status = (int)httpResponseMessage.StatusCode;
                    var reason = httpResponseMessage.ReasonPhrase ?? "Sem motivo informado";
                    var body = await httpResponseMessage.Content.ReadAsStringAsync();

                    response.AddError($"Erro ao enviar dados ({status} {reason})");
                    return response;
                }

                response.Result = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao enviar dados ({ex})");
            }

            return response;
        }
    }
}
