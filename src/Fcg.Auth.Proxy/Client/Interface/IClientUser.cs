using Fcg.Auth.Common;
using Fcg.Auth.Proxy.User.Client.Responses;

namespace Fcg.Auth.Proxy.User.Client.Interface
{
    public interface IClientUser
    {
        Task<Response<CreateUserResponse>> CreateUserAsync(Guid id);
    }
}
