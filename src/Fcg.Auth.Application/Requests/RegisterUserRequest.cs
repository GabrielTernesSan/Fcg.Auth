using Fcg.Auth.Common;
using MediatR;

namespace Fcg.Auth.Application.Requests
{
    public class RegisterUserRequest : IRequest<Response>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
