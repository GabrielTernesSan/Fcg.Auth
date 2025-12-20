using Fcg.Auth.Application.Requests;
using Fcg.Auth.Application.Responses;
using Fcg.Auth.Domain.Repositories;
using MediatR;

namespace Fcg.Auth.Application.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, ExternalResponse>
    {
        private readonly IAuthUserRepository _authUserRepository;

        public DeleteUserHandler(IAuthUserRepository userRepository)
        {
            _authUserRepository = userRepository;
        }

        public async Task<ExternalResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authUserRepository.GetUserByIdAsync(request.Id);

                if (user == null)
                {
                    return new ExternalResponse
                    {
                        Success = false,
                        Message = $"O usuário alvo não foi encontrado."
                    };
                }

                await _authUserRepository.DeleteUserAsync(user.Id);

                return new ExternalResponse
                {
                    Success = true,
                    Message = "Usuário deletado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ExternalResponse
                {
                    Success = false,
                    Message = $"Erro ao deletar usuário: {ex.Message}"
                };
            }
        }
    }
}
