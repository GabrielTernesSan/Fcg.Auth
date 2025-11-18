using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using MediatR;

namespace Fcg.Auth.Application.Handlers
{
    public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleRequest, Response>
    {
        private readonly IAuthUserRepository _authUserRepository;

        public ChangeUserRoleHandler(IAuthUserRepository authUserRepository)
        {
            _authUserRepository = authUserRepository;
        }

        public async Task<Response> Handle(ChangeUserRoleRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var performer = await _authUserRepository.GetUserByIdAsync(request.PerformerId);

            if (performer?.Role != "Admin")
            {
                response.AddError("O usuário não tem permissão para esta ação.");
                return response;
            }

            var targetUser = await _authUserRepository.GetUserByIdAsync(request.TargetUserId);

            if (targetUser == null)
            {
                response.AddError("O usuário não foi encontrado.");
                return response;
            }

            targetUser.SetRole(request.NewRole);

            await _authUserRepository.UpdateUserRoleAsync(targetUser);

            return response;
        }
    }
}
