using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using MediatR;

namespace Fcg.Auth.Application.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, Response>
    {
        private readonly IAuthUserRepository _authUserRepository;

        public DeleteUserHandler(IAuthUserRepository userRepository)
        {
            _authUserRepository = userRepository;
        }

        public async Task<Response> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _authUserRepository.GetUserByIdAsync(request.Id);

            if (user == null)
            {
                response.AddError("O usuário alvo não foi encontrado.");
                return response;
            }

            await _authUserRepository.DeleteUserAsync(user.Id);

            return response;
        }
    }
}
