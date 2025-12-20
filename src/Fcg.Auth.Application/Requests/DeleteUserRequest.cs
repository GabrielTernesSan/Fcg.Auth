using Fcg.Auth.Application.Responses;
using MediatR;

namespace Fcg.Auth.Application.Requests
{
    public class DeleteUserRequest : IRequest<ExternalResponse>
    {
        public Guid Id { get; set; }
    }
}
