using Fcg.Auth.Common;
using Fcg.Auth.Proxy.User.Responses;

namespace Fcg.Auth.Proxy.User.Interface
{
    public interface IClientUser
    {
        Task<Response<CreateUserResponse>> CreateUserAsync(Guid id);
    }
}
