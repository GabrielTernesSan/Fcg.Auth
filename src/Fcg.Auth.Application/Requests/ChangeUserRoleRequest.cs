using Fcg.Auth.Common;
using FluentValidation;
using MediatR;

namespace Fcg.Auth.Application.Requests
{
    public class ChangeUserRoleRequest : IRequest<Response>
    {
        public Guid TargetUserId { get; set; }
        public string NewRole { get; set; } = null!;
    }

    public class ChangeUserRoleRequestValidator : AbstractValidator<ChangeUserRoleRequest>
    {
        public ChangeUserRoleRequestValidator()
        {
            RuleFor(x => x.TargetUserId)
                .NotEmpty().WithMessage("O ID do usuário alvo é obrigatório.");
            RuleFor(x => x.NewRole)
                .NotEmpty().WithMessage("A nova função é obrigatória.");
        }
    }
}
