using Fcg.Auth.Application.Requests;
using Fcg.Auth.Common;
using Fcg.Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleRequest, Response>
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeUserRoleHandler(IAuthUserRepository authUserRepository,
                                 IHttpContextAccessor httpContextAccessor)
    {
        _authUserRepository = authUserRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response> Handle(ChangeUserRoleRequest request, CancellationToken cancellationToken)
    {
        var response = new Response();

        var performerId = Guid.Parse(
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var performer = await _authUserRepository.GetUserByIdAsync(performerId);

        if (performer?.Role != "Admin")
        {
            response.AddError("Você não tem autorização para alterar papéis de usuário.");
            return response;
        }

        if (performer.Id == request.TargetUserId)
        {
            response.AddError("Você não pode alterar seu próprio papel.");
            return response;
        }

        var targetUser = await _authUserRepository.GetUserByIdAsync(request.TargetUserId);

        if (targetUser == null)
        {
            response.AddError("O usuário alvo não foi encontrado.");
            return response;
        }

        targetUser.SetRole(request.NewRole);

        await _authUserRepository.UpdateUserRoleAsync(targetUser);

        return response;
    }
}
