using Fcg.Auth.Common;
using MediatR;

namespace Fcg.Auth.Application.Requests
{
    public class DeleteUserRequest : IRequest<Response>
    {
        public Guid Id { get; set; }
    }
}
