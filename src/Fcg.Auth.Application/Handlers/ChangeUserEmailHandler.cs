using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using MediatR;

namespace Fcg.Auth.Application.Handlers
{
    public class ChangeUserEmailHandler : IRequestHandler<ChangeUserEmailRequest, Response>
    {
        private readonly IAuthUserRepository _authUserRepository;

        public ChangeUserEmailHandler(IAuthUserRepository repository)
        {
            _authUserRepository = repository;
        }

        public async Task<Response> Handle(ChangeUserEmailRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var user = await _authUserRepository.GetUserByIdAsync(request.UserId);

            if (user == null)
            {
                response.AddError("O usuário alvo não foi encontrado.");
                return response;
            }

            user.UpdateEmail(request.Email);

            await _authUserRepository.UpdateUserAsync(user);

            return response;
        }
    }
}
