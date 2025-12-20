using Fcg.Auth.Application.Requests;
using Fcg.Auth.Application.Responses;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using MediatR;

namespace Fcg.Auth.Application.Handlers
{
    public class ChangeUserEmailHandler : IRequestHandler<ChangeUserEmailRequest, ExternalResponse>
    {
        private readonly IAuthUserRepository _authUserRepository;

        public ChangeUserEmailHandler(IAuthUserRepository repository)
        {
            _authUserRepository = repository;
        }

        public async Task<ExternalResponse> Handle(ChangeUserEmailRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authUserRepository.GetUserByIdAsync(request.UserId);

                if (user == null)
                {
                    return new ExternalResponse
                    {
                        Success = false,
                        Message = $"O usuário alvo não foi encontrado."
                    };
                }

                user.UpdateEmail(request.Email);

                await _authUserRepository.UpdateUserAsync(user);

                return new ExternalResponse
                {
                    Success = true,
                    Message = "Usuário atualizado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ExternalResponse
                {
                    Success = false,
                    Message = $"Erro ao atualizar usuário: {ex.Message}"
                };
            }
        }
    }
}
